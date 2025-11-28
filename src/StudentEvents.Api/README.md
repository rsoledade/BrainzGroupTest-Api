Configurações da API - StudentEvents

Este arquivo explica como alterar a string de conexão com o banco de dados, as configurações JWT e a senha padrão dos usuários de teste para o projeto `StudentEvents.Api`.

Arquivos relevantes
- `src/StudentEvents.Api/appsettings.json` – arquivo de configuração principal usado pela aplicação.
- `src/StudentEvents.Api/appsettings.Development.json` – configurações para ambiente de desenvolvimento (ex.: connection string local e senha de teste).

String de conexão (SQL Server)
- Propriedade: `ConnectionStrings:DefaultConnection`
- Exemplo padrão (localhost):
 `Server=localhost;Database=StudentEventsDb;Trusted_Connection=True;MultipleActiveResultSets=true`
- Alternativa local (LocalDB):
 `Server=(localdb)\\MSSQLLocalDB;Database=Students;Integrated Security=True;...`
- Recomendações: não commitar credenciais sensíveis. Para ambientes diferentes, use `appsettings.Development.json`, `appsettings.Production.json` ou variáveis de ambiente.
- Variável de ambiente correspondente: `ConnectionStrings__DefaultConnection` (duplo underscore para separar níveis).

Configurações JWT
- Seção: `JwtSettings` no `appsettings.json`.
- Propriedades:
 - `Key` – chave secreta usada para assinar tokens. Substitua o valor padrão por uma chave forte.
 - `Issuer` – emissor do token (ex.: `StudentEventsApi`).
 - `Audience` – audiência do token (ex.: `StudentEventsApiUsers`).
 - `ExpireMinutes` – tempo de expiração do token em minutos (padrão120).
- Exemplo mínimo no `appsettings.json`:
 ```json
 "JwtSettings": {
 "Key": "sua-chave-secreta-forte-aqui",
 "Issuer": "StudentEventsApi",
 "Audience": "StudentEventsApiUsers",
 "ExpireMinutes":120
 }
 ```
- Para evitar expor a chave em código-fonte, use uma variável de ambiente `JwtSettings__Key` ou o `dotnet user-secrets` em desenvolvimento.

Usuários de teste
- A aplicação insere automaticamente dois usuários de teste na primeira execução, se a tabela `Users` estiver vazia: `admin@school.local` e `userpadrao@school.local`.
- Senha padrão dos usuários de teste (desenvolvimento): configurada em `TestUsers:DefaultPassword` dentro de `appsettings.Development.json` ou via user-secrets.
- Valor padrão atual: `123456`.
- Para alterar a senha de teste sem commitar mudanças ao repositório, use `dotnet user-secrets` para o projeto `StudentEvents.Api`:

```bash
cd src/StudentEvents.Api
dotnet user-secrets init
dotnet user-secrets set "TestUsers:DefaultPassword" "sua-senha-de-teste-aqui"
```

- Você também pode usar variável de ambiente `TestUsers__DefaultPassword` (Windows PowerShell):

```powershell
$env:TestUsers__DefaultPassword = "sua-senha-de-teste-aqui"
```

Comportamento no startup
- Ao iniciar, a aplicação verifica migrations pendentes e aplica-as.
- Em seguida, se a tabela `Users` estiver vazia, insere os dois usuários de teste com a senha configurada.
- Se os usuários já existirem, o seeder é pulado e nada é alterado.

Exemplos de uso (Login + usar token para rotas protegidas)

1) Obter token via Swagger (recomendado durante desenvolvimento):
- Abra Swagger UI: `https://localhost:{porta}/swagger`.
- Use o endpoint `POST /api/auth/login` com JSON:
```json
{
 "email": "admin@school.local",
 "password": "123456"
}
```
- Copie o `token` retornado.
- Clique no botão "Authorize" no Swagger UI e cole: `Bearer <token>` (inclua a palavra `Bearer` seguida de espaço e o token).
- Agora as chamadas aos endpoints protegidos (ex.: `GET /api/students`) devem retornar200 com dados.

2) Usar curl (exemplo):
```bash
TOKEN="<copie_aqui_o_token>"
curl -H "Authorization: Bearer $TOKEN" https://localhost:{porta}/api/students
```

3) Usar Postman:
- Faça POST `/api/auth/login` para obter token.
- Em seguida, nas requisições protegidas adicione header `Authorization` com valor `Bearer <token>`.

Dicas de depuração
- Se receber401, verifique:
 - Se o token não expirou (Veja `exp` do token). Expiração padrão120 minutos.
 - Se `Issuer` e `Audience` do token correspondem aos valores em `appsettings`.
 - Se você colou `Bearer <token>` corretamente no Swagger ou header.

Boas práticas
- Nunca versionar chaves ou senhas reais. Use `dotnet user-secrets` em desenvolvimento e provedores seguros em produção (ex.: Azure Key Vault).

Executando os testes unitários

O repositório contém um projeto de testes em `src/StudentEvents.Tests` com xUnit, Moq e EF Core InMemory. Para executar os testes localmente:

1. Na raiz do repositório, rode:

```bash
dotnet test
```

2. Para executar apenas o projeto de testes:

```bash
dotnet test src/StudentEvents.Tests/StudentEvents.Tests.csproj
```

3. (Opcional) Para gerar cobertura localmente usando coverlet (se tiver o global tool ou integrado a sua IDE), você pode instalar e executar:

```bash
# Exemplo usando coverlet collector integrado ao dotnet test
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

Os testes cobrem as funcionalidades básicas de `AuthService`, `StudentService`, `TokenService` e os controllers `AuthController` e `StudentsController`.

Se quiser, posso adicionar uma pequena rota pública que retorna as claims do token (útil para depuração).