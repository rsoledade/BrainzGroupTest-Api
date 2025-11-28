using System.Text.Json;
using System.Net.Http.Json;
using StudentEvents.Domain.Entities;
using StudentEvents.Infrastructure.Repositories;

namespace StudentEvents.Application.Services
{
    public class GraphSyncService : IGraphSyncService
    {
        private readonly IStudentRepository _repo;
        private readonly IGraphClientFactory _factory;

        public GraphSyncService(IGraphClientFactory factory, IStudentRepository repo)
        {
            _repo = repo;
            _factory = factory;
        }

        public async Task SyncAsync()
        {
            var client = _factory.Create();

            // Fetch users (v1.0) - using $select
            var users = new List<JsonElement>();
            var next = "users?$select=id,displayName,mail,userPrincipalName&$top=999";
            while (!string.IsNullOrEmpty(next))
            {
                var root = await client.GetFromJsonAsync<JsonElement>(next);
                if (root.ValueKind == JsonValueKind.Undefined || root.ValueKind == JsonValueKind.Null) break;

                if (root.TryGetProperty("value", out JsonElement valueElem) && valueElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var u in valueElem.EnumerateArray())
                    {
                        users.Add(u);
                    }
                }

                string? nextLink = null;
                if (root.TryGetProperty("@odata.nextLink", out JsonElement nextElem) && nextElem.ValueKind == JsonValueKind.String)
                {
                    nextLink = nextElem.GetString();
                }

                if (string.IsNullOrEmpty(nextLink)) break;

                if (nextLink.StartsWith("https://graph.microsoft.com/v1.0/", StringComparison.OrdinalIgnoreCase))
                    next = nextLink.Substring("https://graph.microsoft.com/v1.0/".Length);
                else
                    next = nextLink;
            }

            foreach (var u in users)
            {
                var upn = u.TryGetProperty("userPrincipalName", out var upnElem) && upnElem.ValueKind == JsonValueKind.String ? upnElem.GetString() : null;
                if (string.IsNullOrEmpty(upn)) continue;

                var student = new Student
                {
                    DisplayName = u.TryGetProperty("displayName", out var dn) && dn.ValueKind == JsonValueKind.String ? dn.GetString()! : string.Empty,
                    Mail = u.TryGetProperty("mail", out var m) && m.ValueKind == JsonValueKind.String ? m.GetString()! : string.Empty,
                    UserPrincipalName = upn
                };

                await _repo.UpsertStudentAsync(student);
                var dbStudent = (await _repo.GetAllAsync()).FirstOrDefault(x => x.UserPrincipalName == student.UserPrincipalName);
                if (dbStudent == null) continue;

                // fetch events
                var evNext = $"users/{dbStudent.UserPrincipalName}/events?$select=id,subject,start,end,bodyPreview&$top=999";
                var evList = new List<CalendarEvent>();
                while (!string.IsNullOrEmpty(evNext))
                {
                    var evRoot = await client.GetFromJsonAsync<JsonElement>(evNext);
                    if (evRoot.ValueKind == JsonValueKind.Undefined || evRoot.ValueKind == JsonValueKind.Null) break;

                    if (evRoot.TryGetProperty("value", out JsonElement evValue) && evValue.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var ev in evValue.EnumerateArray())
                        {
                            DateTimeOffset start = DateTimeOffset.MinValue;
                            DateTimeOffset end = DateTimeOffset.MinValue;
                            if (ev.TryGetProperty("start", out var s) && s.TryGetProperty("dateTime", out var sdt) && sdt.ValueKind == JsonValueKind.String)
                            {
                                DateTimeOffset.TryParse(sdt.GetString(), out start);
                            }
                            if (ev.TryGetProperty("end", out var e) && e.TryGetProperty("dateTime", out var edt) && edt.ValueKind == JsonValueKind.String)
                            {
                                DateTimeOffset.TryParse(edt.GetString(), out end);
                            }

                            var graphId = ev.TryGetProperty("id", out var idElem) && idElem.ValueKind == JsonValueKind.String ? idElem.GetString() : null;
                            var subject = ev.TryGetProperty("subject", out var subj) && subj.ValueKind == JsonValueKind.String ? subj.GetString() : null;
                            var bodyPreview = ev.TryGetProperty("bodyPreview", out var bp) && bp.ValueKind == JsonValueKind.String ? bp.GetString() : null;

                            evList.Add(new CalendarEvent
                            {
                                GraphId = graphId ?? string.Empty,
                                Subject = subject ?? string.Empty,
                                Start = start,
                                End = end,
                                BodyPreview = bodyPreview ?? string.Empty
                            });
                        }
                    }

                    string? evNextLink = null;
                    if (evRoot.TryGetProperty("@odata.nextLink", out var evNextElem) && evNextElem.ValueKind == JsonValueKind.String)
                    {
                        evNextLink = evNextElem.GetString();
                    }

                    if (string.IsNullOrEmpty(evNextLink)) break;
                    if (evNextLink.StartsWith("https://graph.microsoft.com/v1.0/", StringComparison.OrdinalIgnoreCase))
                        evNext = evNextLink.Substring("https://graph.microsoft.com/v1.0/".Length);
                    else
                        evNext = evNextLink;
                }

                await _repo.UpsertEventsAsync(dbStudent.Id, evList);
            }
        }
    }
}
