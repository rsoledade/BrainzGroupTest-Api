Configurações da API - StudentEvents

Este arquivo explica como alterar a string de conexão com o banco de dados e as configurações JWT para o projeto `StudentEvents.Api`.

Arquivos relevantes
- `src/StudentEvents.Api/appsettings.json` – arquivo de configuração principal usado pela aplicação.

String de conexão (SQL Server)
- Propriedade: `ConnectionStrings:DefaultConnection`
- Exemplo padrão (localhost):
 `Server=localhost;Database=StudentEventsDb;Trusted_Connection=True;MultipleActiveResultSets=true`
- Alternativa local (LocalDB):
 `Server=(localdb)\\mssqllocaldb;Database=StudentEventsDb;Trusted_Connection=True;MultipleActiveResultSets=true`
- Recomendações: não commitar credenciais sensíveis. Para ambientes diferentes, use `appsettings.Development.json`, `appsettings.Production.json` ou variáveis de ambiente.
- Variável de ambiente correspondente: `ConnectionStrings__DefaultConnection` (duplo underscore para separar níveis).

Configurações JWT
- Seção: `JwtSettings` no `appsettings.json`.
- Propriedades:
 - `Key` – chave secreta usada para assinar tokens. Substitua o valor padrão por uma chave forte.
 - `Issuer` – emissor do token (ex.: `StudentEventsApi`).
 - `Audience` – audiência do token (ex.: `StudentEventsApiUsers`).
 - `ExpireMinutes` – tempo de expiração do token em minutos.
- Exemplo mínimo no `appsettings.json`:
 ```json
 "JwtSettings": {
 "Key": "sua-chave-secreta-forte-aqui",
 "Issuer": "StudentEventsApi",
 "Audience": "StudentEventsApiUsers",
 "ExpireMinutes":60
 }
 ```
- Para evitar expor a chave em código-fonte, use uma variável de ambiente `JwtSettings__Key` ou o `dotnet user-secrets` em desenvolvimento.

Boas práticas
- Nunca versionar chaves secretas ou credenciais.
- Use variáveis de ambiente ou provedores de segredo (Azure Key Vault, AWS Secrets Manager, etc.) em produção.
- Teste a configuração de JWT e a string de conexão localmente antes de subir para ambientes remotos.

Se precisar, posso adicionar exemplos de `appsettings.Development.json` e instruções para usar `dotnet user-secrets`.