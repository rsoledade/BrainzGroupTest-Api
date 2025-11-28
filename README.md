# StudentEvents - Configuração local e secrets

Este projeto lê secrets em tempo de execução a partir de `IConfiguration`. Para desenvolvimento local recomendamos o uso de `dotnet user-secrets`. A aplicação espera as seguintes chaves de configuração estarem disponíveis em tempo de execução (via user-secrets, variáveis de ambiente ou um provedor como Azure Key Vault):

- `Graph:ClientId` — id do aplicativo (client) no Azure AD
- `Graph:ClientSecret` — secret do client no Azure AD (sensível)
- `Graph:TenantId` — id do tenant do Azure AD
- `JwtSettings:Key` — chave simétrica para assinar JWTs
- `JwtSettings:Issuer` e `JwtSettings:Audience` — valores opcionais de emissor/audiência

A classe `GraphClientFactory` lê as chaves `Graph` via `IConfiguration` e lançará um erro claro caso algum valor esteja ausente. Veja `StudentEvents.Application/Services/GraphClientFactory.cs`.

---

## Checklist rápido para o avaliador

Siga estes passos para executar a aplicação localmente e conectá-la ao Microsoft Graph.

1. Clone o repositório e abra um terminal.
2. Entre na pasta do projeto da API (arquivo `StudentEvents.Api.csproj`):

```bash
cd src/StudentEvents.Api
```

3. Verifique se o SDK .NET8 está instalado: `dotnet --version` (deve começar com `8`).

4. Inicialize o user-secrets (uma vez por projeto):

```bash
dotnet user-secrets init
```

> Observação: o projeto já contém um `UserSecretsId`. Se já estiver inicializado, este comando não altera nada.

5. Adicione os secrets necessários (substitua pelos valores do seu app no Azure AD):

```bash
dotnet user-secrets set "Graph:ClientId" "<CLIENT_ID>"
dotnet user-secrets set "Graph:ClientSecret" "<CLIENT_SECRET>"
dotnet user-secrets set "Graph:TenantId" "<TENANT_ID>"

# Opcional: configurações do JWT usadas pelo AuthController
dotnet user-secrets set "JwtSettings:Key" "<JWT_SIGNING_KEY>"
dotnet user-secrets set "JwtSettings:Issuer" "<ISSUER>"
dotnet user-secrets set "JwtSettings:Audience" "<AUDIENCE>"
```

6. (Opcional) Verifique os secrets:

```bash
dotnet user-secrets list
```

7. Execute a API localmente:

```bash
dotnet run --project src/StudentEvents.Api
```

8. Em desenvolvimento, o Swagger ficará disponível em `https://localhost:{porta}/swagger`.

---

## Alternativa: variáveis de ambiente (containers / CI / produção)

Defina variáveis de ambiente usando `__` para representar `:` nas chaves de configuração. Exemplos:

- Linux / macOS (bash):

```bash
export Graph__ClientId="<CLIENT_ID>"
export Graph__ClientSecret="<CLIENT_SECRET>"
export Graph__TenantId="<TENANT_ID>"
export JwtSettings__Key="<JWT_SIGNING_KEY>"
```

- Windows PowerShell:

```powershell
$env:Graph__ClientId = "<CLIENT_ID>"
$env:Graph__ClientSecret = "<CLIENT_SECRET>"
$env:Graph__TenantId = "<TENANT_ID>"
$env:JwtSettings__Key = "<JWT_SIGNING_KEY>"
```

O `IConfiguration` do ASP.NET Core resolvê-las como `Graph:ClientId`, etc.

---

## CI / GitHub Actions

Armazene secrets no repositório (Settings ? Secrets) e injete-os no workflow como variáveis de ambiente. Exemplo de snippet:

```yaml
env:
 Graph__ClientId: ${{ secrets.GRAPH_CLIENT_ID }}
 Graph__ClientSecret: ${{ secrets.GRAPH_CLIENT_SECRET }}
 Graph__TenantId: ${{ secrets.GRAPH_TENANT_ID }}
 JwtSettings__Key: ${{ secrets.JWT_KEY }}
```

---

## Notas de segurança

- Nunca commit secrets ao repositório. Se um secret foi exposto, regenere-o imediatamente.
- Para produção, prefira um gerenciador de secrets (ex.: Azure Key Vault) e configure um provider de configuração para que o `IConfiguration` leia os secrets diretamente.

---

## Onde o código lê os secrets

- `StudentEvents.Application/Services/GraphClientFactory.cs` usa `IConfiguration.GetValue<string>("Graph:ClientId")`, etc.
- `StudentEvents.Api/Program.cs` configura os providers de configuração e `AuthController` usa `JwtSettings:Key` para assinar tokens.

Se quiser, posso adicionar um workflow do GitHub Actions que demonstra como passar os secrets e executar um build CI.
