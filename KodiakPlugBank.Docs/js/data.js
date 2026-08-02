window.KODIAK_DOCS = {
  ambientes: [
    { id: "homologacao", nome: "Homologação", base: "http://localhost:5299" },
    { id: "producao", nome: "Produção", base: "https://api.kodiakplugbank.com.br" }
  ],

  secoes: [
    {
      id: "visao-geral",
      tipo: "html",
      titulo: "Visão Geral",
      menu: "Visão Geral",
      html: `
        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-bank me-2 text-info"></i>Sobre esta documentação</h5>
            <p class="mb-2">
              Documentação da <strong>Kodiak PlugBank API</strong>, a camada intermediária que permite ao
              KodiakERP cadastrar <strong>pagadores</strong> e, futuramente, <strong>contas</strong> e
              <strong>extratos bancários</strong> via <em>Open Finance</em>, integrando com a
              <a href="https://docs.pagamentobancario.com.br/" target="_blank" class="link-info">PlugBank (TecnoSped)</a>.
            </p>
            <p class="mb-2">
              A API recebe as chamadas do KodiakERP e as repassa à PlugBank, mantendo um espelho local no banco
              de dados (identificação do pagador e <code>tokenPayer</code>).
            </p>
            <p class="mb-0">O menu lateral permite navegar pelos módulos. Conta e Extrato serão documentados nas próximas etapas.</p>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-hdd-network me-2 text-info"></i>URL base da API</h5>
            <table class="table align-middle mb-0">
              <thead>
                <tr><th>Ambiente</th><th>Base da API Kodiak</th><th>Base PlugBank (uso interno)</th></tr>
              </thead>
              <tbody>
                <tr>
                  <td>Homologação</td>
                  <td><code>{base}</code></td>
                  <td><code>https://staging.pagamentobancario.com.br</code></td>
                </tr>
                <tr>
                  <td>Produção</td>
                  <td><code>https://api.kodiakplugbank.com.br</code></td>
                  <td><code>https://api.pagamentobancario.com.br</code></td>
                </tr>
              </tbody>
            </table>
            <div class="aviso aviso-info mt-3 mb-0">
              <i class="bi bi-info-circle me-1"></i>
              Use o <strong>seletor de ambiente</strong> no topo para trocar a URL base dos exemplos.
              A URL de produção é um placeholder e deve ser ajustada em <code>KodiakPlugBank.Docs/js/data.js</code>.
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-list-ul me-2 text-info"></i>Endpoints — Pagador</h5>
            <ul class="list-unstyled indice-endpoints mb-0">
              <li class="mb-1"><span class="badge-metodo badge text-bg-success me-2">POST</span><a href="#/pagador/criar">/api/v1/payer</a> — cadastra um pagador na PlugBank e no banco local.</li>
              <li class="mb-1"><span class="badge-metodo badge text-bg-primary me-2">GET</span><a href="#/pagador/consultar">/api/v1/payer</a> — consulta um pagador na PlugBank pelo CPF/CNPJ.</li>
              <li class="mb-1"><span class="badge-metodo badge text-bg-primary me-2">GET</span><a href="#/pagador/listar">/api/v1/payer/list</a> — lista os pagadores cadastrados na PlugBank.</li>
              <li class="mb-1"><span class="badge-metodo badge text-bg-warning me-2">PUT</span><a href="#/pagador/atualizar">/api/v1/payer</a> — atualiza os dados do pagador na PlugBank e no banco local.</li>
              <li class="mb-1"><span class="badge-metodo badge text-bg-danger me-2">DELETE</span><a href="#/pagador/desativar">/api/v1/payer/{tokenPayer}</a> — desativa um pagador na PlugBank e no banco local.</li>
            </ul>
          </div>
        </div>
      `
    },

    {
      id: "autenticacao",
      tipo: "html",
      titulo: "Autenticação e Headers",
      menu: "Autenticação",
      html: `
        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-shield-lock me-2 text-info"></i>Autenticação da API</h5>
            <p class="mb-3">
              Todas as rotas exigem o header <code>X-Api-Key</code> com a <strong>apikey fixa do KodiakERP</strong>.
              Sem ela a API responde <code>401</code>.
            </p>
            <div class="aviso aviso-perigo mb-3">
              <i class="bi bi-exclamation-triangle me-1"></i>
              Não compartilhe a apikey. Nos exemplos abaixo, substitua <code>SUA_APIKEY</code> pela chave fornecida ao cliente.
            </div>
            <p class="mb-0">
              Em desenvolvimento, a UI do Swagger (somente no ambiente de desenvolvimento) já possui o botão
              <strong>Authorize</strong> para informar a chave e testar os endpoints.
            </p>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-arrow-left-right me-2 text-info"></i>Headers utilizados</h5>
            <table class="table align-middle mb-0">
              <thead>
                <tr><th>Header</th><th>Obrigatório</th><th>Descrição</th></tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>X-Api-Key</code></td>
                  <td><span class="obrigatorio">Sim</span> (todas as rotas)</td>
                  <td>Apikey fixa do KodiakERP. Autentica a chamada na Kodiak API.</td>
                </tr>
                <tr>
                  <td><code>payercpfcnpj</code></td>
                  <td><span class="obrigatorio">Sim</span> (consulta, atualização e desativação)</td>
                  <td>CPF/CNPJ do pagador (somente números). Identifica o pagador na PlugBank.</td>
                </tr>
                <tr>
                  <td><code>Content-Type</code></td>
                  <td>Somente quando houver body</td>
                  <td><code>application/json</code>.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-gear me-2 text-info"></i>Credenciais da PlugBank (uso interno)</h5>
            <p class="mb-2">
              Os headers <code>cnpjsh</code> e <code>tokensh</code> usados na integração com a PlugBank
              <strong>não devem ser enviados pelo KodiakERP</strong>. Eles são lidos pelo servidor a partir das
              variáveis de ambiente:
            </p>
            <ul class="mb-0">
              <li><code>cnpjsh</code> → variável de ambiente <code>KODIAK_PLUGBANK_SH</code></li>
              <li><code>tokensh</code> → variável de ambiente <code>KODIAK_PLUGBANK</code></li>
            </ul>
            <div class="aviso aviso-info mt-3 mb-0">
              <i class="bi bi-info-circle me-1"></i>
              O comportamento das respostas também depende da liberação do <strong>IP do servidor</strong> na
              TecnoSped. Enquanto o IP não estiver liberado, as chamadas à PlugBank retornam <code>403</code>.
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h5 class="mb-3"><i class="bi bi-speedometer2 me-2 text-info"></i>Limites de requisição (rate limiting)</h5>
            <table class="table align-middle mb-0">
              <thead>
                <tr><th>Política</th><th>Limite</th><th>Rotas</th></tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>global</code></td>
                  <td>100 requisições / 10 s por IP</td>
                  <td>Todas as rotas</td>
                </tr>
                <tr>
                  <td><code>bootstrap</code></td>
                  <td>10 requisições / 60 s por IP</td>
                  <td><code>POST /api/v1/payer</code></td>
                </tr>
              </tbody>
            </table>
            <p class="mb-0 mt-3 small text-secondary">
              Ao exceder o limite, a API responde <code>429</code> com o header <code>Retry-After</code>.
            </p>
          </div>
        </div>
      `
    },

    {
      id: "pagador",
      tipo: "grupo",
      titulo: "Pagador",
      menu: "Pagador",
      descricao: "Endpoints para cadastrar, consultar, listar, atualizar e desativar pagadores.",
      endpoints: [
        {
          id: "criar",
          metodo: "POST",
          rota: "/api/v1/payer",
          nome: "Criar pagador",
          resumo: "Cadastra um pagador na PlugBank e o grava no banco local, armazenando o tokenPayer.",
          oqueFaz: "O cadastro é feito primeiro na PlugBank. Em caso de sucesso, o pagador é persistido localmente com o campo token (tokenPayer) retornado — ele é necessário para rotas futuras (conta, extrato e desativação).",
          headers: [
            { nome: "X-Api-Key", obrigatorio: true, descricao: "Apikey fixa do KodiakERP." },
            { nome: "Content-Type", obrigatorio: true, descricao: "application/json." }
          ],
          campos: [
            { nome: "name", tipo: "string", obrigatorio: true, descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", obrigatorio: false, descricao: "E-mail do pagador." },
            { nome: "cpfCnpj", tipo: "string", obrigatorio: true, descricao: "CPF ou CNPJ, somente números (ex.: 12345678909)." },
            { nome: "ddaActived", tipo: "boolean", obrigatorio: false, descricao: "Ativa o DDA (débito direto autorizado)." },
            { nome: "statementActived", tipo: "boolean", obrigatorio: false, descricao: "Ativa o extrato bancário (Open Finance)." },
            { nome: "street", tipo: "string", obrigatorio: true, descricao: "Logradouro (rua/avenida)." },
            { nome: "neighborhood", tipo: "string", obrigatorio: true, descricao: "Bairro." },
            { nome: "addressNumber", tipo: "string", obrigatorio: false, descricao: "Número do endereço." },
            { nome: "addressComplement", tipo: "string", obrigatorio: false, descricao: "Complemento do endereço." },
            { nome: "city", tipo: "string", obrigatorio: true, descricao: "Cidade." },
            { nome: "state", tipo: "string", obrigatorio: true, descricao: "UF (ex.: SP)." },
            { nome: "zipcode", tipo: "string", obrigatorio: true, descricao: "CEP (somente números)." },
            { nome: "accounts", tipo: "array de conta", obrigatorio: false, descricao: "Contas bancárias do pagador. Cada item possui: bankCode, agency, agencyDigit, accountNumber, accountNumberDigit, accountDac, convenioAgency, convenioNumber, remessaSequential." },
            { nome: "chaveKodiakExtrato", tipo: "string", obrigatorio: false, descricao: "Chave interna do Kodiak. Apenas armazenada; não é usada para validação." }
          ],
          bodyExemplo: `{
  "name": "João da Silva",
  "email": "joao@exemplo.com",
  "cpfCnpj": "12345678909",
  "ddaActived": false,
  "statementActived": true,
  "street": "Rua das Flores",
  "neighborhood": "Centro",
  "addressNumber": "100",
  "addressComplement": "Sala 2",
  "city": "São Paulo",
  "state": "SP",
  "zipcode": "01001000",
  "accounts": [
    {
      "bankCode": "001",
      "agency": "0001",
      "accountNumber": "12345",
      "accountNumberDigit": "6"
    }
  ],
  "chaveKodiakExtrato": null
}`,
          respostaStatus: "201 Created",
          respostaDescricao: "Pagador criado na PlugBank e registrado localmente. O campo token é o tokenPayer para uso futuro.",
          respostaExemplo: `{
  "id": 1,
  "nome": "João da Silva",
  "email": "joao@exemplo.com",
  "cpfCnpj": "12345678909",
  "chaveKodiakExtrato": "",
  "token": "67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b",
  "statementAtivado": true
}`,
          respostaCampos: [
            { nome: "id", tipo: "integer", descricao: "Identificador local do pagador." },
            { nome: "nome", tipo: "string", descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", descricao: "E-mail do pagador." },
            { nome: "cpfCnpj", tipo: "string", descricao: "CPF/CNPJ do pagador." },
            { nome: "chaveKodiakExtrato", tipo: "string", descricao: "Chave interna do Kodiak (apenas dado)." },
            { nome: "token", tipo: "string", descricao: "tokenPayer retornado pela PlugBank. Guarde-o." },
            { nome: "statementAtivado", tipo: "boolean", descricao: "Indica se o extrato está ativo." }
          ],
          erros: [
            { status: "400", mensagem: "Name e CpfCnpj são obrigatórios.", descricao: "Corpo inválido ou campos obrigatórios ausentes." },
            { status: "401", mensagem: "Não autenticado.", descricao: "X-Api-Key ausente ou inválida." },
            { status: "409", mensagem: "Já existe pagador com o CPF/CNPJ ...", descricao: "O CPF/CNPJ informado já está cadastrado." },
            { status: "422", mensagem: "Mensagem da PlugBank.", descricao: "A PlugBank recusou o cadastro por validação." },
            { status: "429", mensagem: "Rate limit excedido.", descricao: "Excedeu 10 cadastros por minuto por IP." }
          ],
          comoUsar: [
            "Envie o POST para /api/v1/payer com o header X-Api-Key e o corpo JSON com os dados do pagador.",
            "Guarde o campo token (tokenPayer) retornado na resposta — ele será usado na desativação e nas rotas de conta e extrato.",
            "Não reutilize o mesmo CPF/CNPJ em dois cadastros: a API responde 409."
          ],
          exemplos: {
            curl: `curl -X POST {base}/api/v1/payer \\
  -H "X-Api-Key: SUA_APIKEY" \\
  -H "Content-Type: application/json" \\
  -d '{
    "name": "João da Silva",
    "email": "joao@exemplo.com",
    "cpfCnpj": "12345678909",
    "statementActived": true,
    "street": "Rua das Flores",
    "neighborhood": "Centro",
    "addressNumber": "100",
    "city": "São Paulo",
    "state": "SP",
    "zipcode": "01001000"
  }'`,
            csharp: `using System.Net.Http.Json;

using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "SUA_APIKEY");

var payload = new
{
    name = "João da Silva",
    email = "joao@exemplo.com",
    cpfCnpj = "12345678909",
    statementActived = true,
    street = "Rua das Flores",
    neighborhood = "Centro",
    addressNumber = "100",
    city = "São Paulo",
    state = "SP",
    zipcode = "01001000"
};

var resposta = await client.PostAsJsonAsync("{base}/api/v1/payer", payload);
var corpo = await resposta.Content.ReadAsStringAsync();
Console.WriteLine(corpo);`,
            powershell: `$body = @{
    name             = "João da Silva"
    email            = "joao@exemplo.com"
    cpfCnpj          = "12345678909"
    statementActived = $true
    street           = "Rua das Flores"
    neighborhood     = "Centro"
    addressNumber    = "100"
    city             = "São Paulo"
    state            = "SP"
    zipcode          = "01001000"
} | ConvertTo-Json

Invoke-RestMethod -Uri "{base}/api/v1/payer" \`
  -Method Post \`
  -Headers @{ "X-Api-Key" = "SUA_APIKEY" } \`
  -ContentType "application/json" \`
  -Body $body`
          }
        },

        {
          id: "consultar",
          metodo: "GET",
          rota: "/api/v1/payer",
          nome: "Consultar pagador",
          resumo: "Consulta um pagador na PlugBank usando o CPF/CNPJ no header payercpfcnpj.",
          oqueFaz: "A consulta é feita diretamente na PlugBank (não no banco local). O pagador é identificado pelo CPF/CNPJ informado no header payercpfcnpj. A resposta traz o token (tokenPayer) e a situação atual do cadastro na PlugBank.",
          headers: [
            { nome: "X-Api-Key", obrigatorio: true, descricao: "Apikey fixa do KodiakERP." },
            { nome: "payercpfcnpj", obrigatorio: true, descricao: "CPF/CNPJ do pagador (somente números)." }
          ],
          campos: [],
          bodyExemplo: null,
          respostaStatus: "200 OK",
          respostaDescricao: "Dados do pagador obtidos na PlugBank.",
          respostaExemplo: `{
  "name": "João da Silva",
  "email": "joao@exemplo.com",
  "status": 1,
  "active": true,
  "cpfCnpj": "12345678909",
  "statementActived": true,
  "accounts": [
    {
      "bankCode": "001",
      "accountHash": "abc123",
      "agency": "0001",
      "accountNumber": "12345",
      "accountNumberDigit": "6"
    }
  ],
  "street": "Rua das Flores",
  "neighborhood": "Centro",
  "addressNumber": "100",
  "city": "São Paulo",
  "state": "SP",
  "zipcode": "01001000",
  "token": "67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b",
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-01T10:00:00Z"
}`,
          respostaCampos: [
            { nome: "name", tipo: "string", descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", descricao: "E-mail do pagador." },
            { nome: "status", tipo: "integer", descricao: "Status do pagador na PlugBank." },
            { nome: "active", tipo: "boolean", descricao: "Se o pagador está ativo." },
            { nome: "cpfCnpj", tipo: "string", descricao: "CPF/CNPJ do pagador." },
            { nome: "statementActived", tipo: "boolean", descricao: "Se o extrato está ativo." },
            { nome: "accounts", tipo: "array", descricao: "Contas associadas ao pagador." },
            { nome: "street / neighborhood / addressNumber / addressComplement / city / state / zipcode", tipo: "string", descricao: "Endereço do pagador." },
            { nome: "token", tipo: "string", descricao: "tokenPayer da PlugBank." },
            { nome: "createdAt / updatedAt", tipo: "string (data)", descricao: "Datas de criação e atualização." }
          ],
          erros: [
            { status: "401", mensagem: "Header payercpfcnpj não informado.", descricao: "O header payercpfcnpj é obrigatório nesta rota." },
            { status: "401", mensagem: "Não autenticado.", descricao: "X-Api-Key ausente ou inválida." },
            { status: "404", mensagem: "Mensagem da PlugBank.", descricao: "Pagador não encontrado para o CPF/CNPJ informado." },
            { status: "403", mensagem: "Mensagem da PlugBank.", descricao: "IP do servidor não liberado na TecnoSped." },
            { status: "422", mensagem: "Mensagem da PlugBank.", descricao: "A PlugBank recusou a consulta por validação." }
          ],
          comoUsar: [
            "Informe o CPF/CNPJ do pagador no header payercpfcnpj (sem máscara).",
            "Envie o GET para /api/v1/payer com os headers X-Api-Key e payercpfcnpj.",
            "A resposta é espelho da PlugBank — use o campo token (tokenPayer) nas rotas que exigem identificação do pagador."
          ],
          exemplos: {
            curl: `curl {base}/api/v1/payer \\
  -H "X-Api-Key: SUA_APIKEY" \\
  -H "payercpfcnpj: 12345678909"`,
            csharp: `using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "SUA_APIKEY");
client.DefaultRequestHeaders.Add("payercpfcnpj", "12345678909");

var resposta = await client.GetAsync("{base}/api/v1/payer");
var corpo = await resposta.Content.ReadAsStringAsync();
Console.WriteLine(corpo);`,
            powershell: `Invoke-RestMethod -Uri "{base}/api/v1/payer" \`
  -Method Get \`
  -Headers @{
    "X-Api-Key"    = "SUA_APIKEY"
    "payercpfcnpj" = "12345678909"
  }`
          }
        },

        {
          id: "listar",
          metodo: "GET",
          rota: "/api/v1/payer/list",
          nome: "Listar pagadores",
          resumo: "Lista os pagadores cadastrados na PlugBank.",
          oqueFaz: "Retorna a lista de pagadores direto da PlugBank. Não exige o header payercpfcnpj. Atenção ao campo stamentActived: o nome com erro de digitação (stamentActived) é o mesmo retornado pela PlugBank e é mantido na resposta.",
          headers: [
            { nome: "X-Api-Key", obrigatorio: true, descricao: "Apikey fixa do KodiakERP." }
          ],
          campos: [],
          bodyExemplo: null,
          respostaStatus: "200 OK",
          respostaDescricao: "Lista de pagadores obtida na PlugBank.",
          respostaExemplo: `{
  "payers": [
    {
      "name": "João da Silva",
      "email": "joao@exemplo.com",
      "active": true,
      "cpfCnpj": "12345678909",
      "token": "67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b",
      "street": "Rua das Flores",
      "neighborhood": "Centro",
      "city": "São Paulo",
      "state": "SP",
      "zipcode": "01001000",
      "createdAt": "2026-07-01T10:00:00Z",
      "updatedAt": "2026-07-01T10:00:00Z",
      "stamentActived": true
    }
  ]
}`,
          respostaCampos: [
            { nome: "payers", tipo: "array", descricao: "Lista de pagadores." },
            { nome: "name", tipo: "string", descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", descricao: "E-mail do pagador." },
            { nome: "active", tipo: "boolean", descricao: "Se o pagador está ativo." },
            { nome: "cpfCnpj", tipo: "string", descricao: "CPF/CNPJ do pagador." },
            { nome: "token", tipo: "string", descricao: "tokenPayer da PlugBank." },
            { nome: "street / neighborhood / addressNumber / addressComplement / city / state / zipcode", tipo: "string", descricao: "Endereço do pagador." },
            { nome: "createdAt / updatedAt", tipo: "string (data)", descricao: "Datas de criação e atualização." },
            { nome: "stamentActived", tipo: "boolean", descricao: "Se o extrato está ativo (typo oficial da PlugBank, mantido)." }
          ],
          erros: [
            { status: "401", mensagem: "Não autenticado.", descricao: "X-Api-Key ausente ou inválida." },
            { status: "403", mensagem: "Mensagem da PlugBank.", descricao: "IP do servidor não liberado na TecnoSped." },
            { status: "422", mensagem: "Mensagem da PlugBank.", descricao: "A PlugBank recusou a listagem por validação." }
          ],
          comoUsar: [
            "Envie o GET para /api/v1/payer/list apenas com o header X-Api-Key.",
            "Use o campo token de cada item para identificar o tokenPayer.",
            "Não há paginação nesta versão — a resposta traz a lista completa."
          ],
          exemplos: {
            curl: `curl {base}/api/v1/payer/list \\
  -H "X-Api-Key: SUA_APIKEY"`,
            csharp: `using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "SUA_APIKEY");

var resposta = await client.GetAsync("{base}/api/v1/payer/list");
var corpo = await resposta.Content.ReadAsStringAsync();
Console.WriteLine(corpo);`,
            powershell: `Invoke-RestMethod -Uri "{base}/api/v1/payer/list" \`
  -Method Get \`
  -Headers @{ "X-Api-Key" = "SUA_APIKEY" }`
          }
        },

        {
          id: "atualizar",
          metodo: "PUT",
          rota: "/api/v1/payer",
          nome: "Atualizar pagador",
          resumo: "Atualiza os dados do pagador na PlugBank e sincroniza o banco local.",
          oqueFaz: "O pagador é identificado pelo CPF/CNPJ do header payercpfcnpj. Se o corpo informar um CPF/CNPJ diferente, a API valida se o novo documento não pertence a outro pagador local. Após a atualização na PlugBank, o cadastro local é sincronizado.",
          headers: [
            { nome: "X-Api-Key", obrigatorio: true, descricao: "Apikey fixa do KodiakERP." },
            { nome: "payercpfcnpj", obrigatorio: true, descricao: "CPF/CNPJ atual do pagador (somente números)." },
            { nome: "Content-Type", obrigatorio: true, descricao: "application/json." }
          ],
          campos: [
            { nome: "name", tipo: "string", obrigatorio: true, descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", obrigatorio: false, descricao: "E-mail do pagador." },
            { nome: "cpfCnpj", tipo: "string", obrigatorio: true, descricao: "CPF ou CNPJ (pode ser igual ao atual ou um novo documento)." },
            { nome: "ddaActived", tipo: "boolean", obrigatorio: false, descricao: "Ativa o DDA." },
            { nome: "statementActived", tipo: "boolean", obrigatorio: false, descricao: "Ativa o extrato bancário." },
            { nome: "street", tipo: "string", obrigatorio: true, descricao: "Logradouro." },
            { nome: "neighborhood", tipo: "string", obrigatorio: true, descricao: "Bairro." },
            { nome: "addressNumber", tipo: "string", obrigatorio: false, descricao: "Número do endereço." },
            { nome: "addressComplement", tipo: "string", obrigatorio: false, descricao: "Complemento." },
            { nome: "city", tipo: "string", obrigatorio: true, descricao: "Cidade." },
            { nome: "state", tipo: "string", obrigatorio: true, descricao: "UF." },
            { nome: "zipcode", tipo: "string", obrigatorio: true, descricao: "CEP." },
            { nome: "accounts", tipo: "array de conta", obrigatorio: false, descricao: "Contas bancárias (mesma estrutura do cadastro)." }
          ],
          bodyExemplo: `{
  "name": "João da Silva Santos",
  "email": "joao.santos@exemplo.com",
  "cpfCnpj": "12345678909",
  "statementActived": true,
  "street": "Rua das Flores",
  "neighborhood": "Centro",
  "addressNumber": "200",
  "city": "São Paulo",
  "state": "SP",
  "zipcode": "01001000"
}`,
          respostaStatus: "200 OK",
          respostaDescricao: "Pagador atualizado na PlugBank e no banco local.",
          respostaExemplo: `{
  "id": 1,
  "nome": "João da Silva Santos",
  "email": "joao.santos@exemplo.com",
  "cpfCnpj": "12345678909",
  "chaveKodiakExtrato": "",
  "token": "67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b",
  "statementAtivado": true
}`,
          respostaCampos: [
            { nome: "id", tipo: "integer", descricao: "Identificador local do pagador." },
            { nome: "nome", tipo: "string", descricao: "Nome do pagador." },
            { nome: "email", tipo: "string", descricao: "E-mail do pagador." },
            { nome: "cpfCnpj", tipo: "string", descricao: "CPF/CNPJ do pagador." },
            { nome: "chaveKodiakExtrato", tipo: "string", descricao: "Chave interna do Kodiak (apenas dado)." },
            { nome: "token", tipo: "string", descricao: "tokenPayer retornado pela PlugBank." },
            { nome: "statementAtivado", tipo: "boolean", descricao: "Indica se o extrato está ativo." }
          ],
          erros: [
            { status: "400", mensagem: "Name e CpfCnpj são obrigatórios.", descricao: "Corpo inválido ou campos obrigatórios ausentes." },
            { status: "401", mensagem: "Header payercpfcnpj não informado.", descricao: "O header payercpfcnpj é obrigatório nesta rota." },
            { status: "401", mensagem: "Não autenticado.", descricao: "X-Api-Key ausente ou inválida." },
            { status: "404", mensagem: "Pagador não encontrado para o payercpfcnpj ...", descricao: "Não existe cadastro local para o CPF/CNPJ do header." },
            { status: "409", mensagem: "Já existe pagador com o CPF/CNPJ ...", descricao: "O novo CPF/CNPJ informado no corpo já pertence a outro pagador." },
            { status: "422", mensagem: "Mensagem da PlugBank.", descricao: "A PlugBank recusou a atualização por validação." }
          ],
          comoUsar: [
            "Informe o CPF/CNPJ atual do pagador no header payercpfcnpj.",
            "Envie no corpo apenas os campos que deseja atualizar (os demais são mantidos).",
            "Se for trocar o CPF/CNPJ, garanta que ele não esteja em uso por outro pagador (senão, 409)."
          ],
          exemplos: {
            curl: `curl -X PUT {base}/api/v1/payer \\
  -H "X-Api-Key: SUA_APIKEY" \\
  -H "payercpfcnpj: 12345678909" \\
  -H "Content-Type: application/json" \\
  -d '{
    "name": "João da Silva Santos",
    "email": "joao.santos@exemplo.com",
    "cpfCnpj": "12345678909",
    "statementActived": true,
    "street": "Rua das Flores",
    "neighborhood": "Centro",
    "addressNumber": "200",
    "city": "São Paulo",
    "state": "SP",
    "zipcode": "01001000"
  }'`,
            csharp: `using System.Net.Http.Json;

using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "SUA_APIKEY");
client.DefaultRequestHeaders.Add("payercpfcnpj", "12345678909");

var payload = new
{
    name = "João da Silva Santos",
    email = "joao.santos@exemplo.com",
    cpfCnpj = "12345678909",
    statementActived = true,
    street = "Rua das Flores",
    neighborhood = "Centro",
    addressNumber = "200",
    city = "São Paulo",
    state = "SP",
    zipcode = "01001000"
};

var resposta = await client.PutAsJsonAsync("{base}/api/v1/payer", payload);
var corpo = await resposta.Content.ReadAsStringAsync();
Console.WriteLine(corpo);`,
            powershell: `$body = @{
    name             = "João da Silva Santos"
    email            = "joao.santos@exemplo.com"
    cpfCnpj          = "12345678909"
    statementActived = $true
    street           = "Rua das Flores"
    neighborhood     = "Centro"
    addressNumber    = "200"
    city             = "São Paulo"
    state            = "SP"
    zipcode          = "01001000"
} | ConvertTo-Json

Invoke-RestMethod -Uri "{base}/api/v1/payer" \`
  -Method Put \`
  -Headers @{
    "X-Api-Key"    = "SUA_APIKEY"
    "payercpfcnpj" = "12345678909"
  } \`
  -ContentType "application/json" \`
  -Body $body`
          }
        },

        {
          id: "desativar",
          metodo: "DELETE",
          rota: "/api/v1/payer/{tokenPayer}",
          nome: "Desativar pagador",
          resumo: "Desativa um pagador na PlugBank e marca como inativo no banco local.",
          oqueFaz: "O pagador é identificado localmente pelo CPF/CNPJ do header payercpfcnpj e, na PlugBank, pelo tokenPayer informado na URL. Após a desativação na PlugBank, o cadastro local é marcado com ativo = false.",
          headers: [
            { nome: "X-Api-Key", obrigatorio: true, descricao: "Apikey fixa do KodiakERP." },
            { nome: "payercpfcnpj", obrigatorio: true, descricao: "CPF/CNPJ do pagador (somente números)." }
          ],
          campos: [
            { nome: "tokenPayer", tipo: "string (path)", obrigatorio: true, descricao: "Token retornado pela PlugBank no cadastro/consulta (campo token)." }
          ],
          bodyExemplo: null,
          respostaStatus: "200 OK",
          respostaDescricao: "Pagador desativado na PlugBank e marcado como inativo localmente.",
          respostaExemplo: `{
  "active": false,
  "message": "Pagador desativado com sucesso",
  "payer": {
    "name": "João da Silva"
  }
}`,
          respostaCampos: [
            { nome: "active", tipo: "boolean", descricao: "false após a desativação." },
            { nome: "message", tipo: "string", descricao: "Mensagem retornada pela PlugBank." },
            { nome: "payer", tipo: "object", descricao: "Objeto com o campo name (nome do pagador)." }
          ],
          erros: [
            { status: "401", mensagem: "Header payercpfcnpj não informado.", descricao: "O header payercpfcnpj é obrigatório nesta rota." },
            { status: "401", mensagem: "Não autenticado.", descricao: "X-Api-Key ausente ou inválida." },
            { status: "404", mensagem: "Pagador não encontrado para o payercpfcnpj ...", descricao: "Não existe cadastro local para o CPF/CNPJ do header." },
            { status: "404", mensagem: "Mensagem da PlugBank.", descricao: "tokenPayer não encontrado na PlugBank." },
            { status: "403", mensagem: "Mensagem da PlugBank.", descricao: "IP do servidor não liberado na TecnoSped." }
          ],
          comoUsar: [
            "Obtenha o tokenPayer (campo token) — ele é retornado no cadastro, na consulta e na listagem.",
            "Informe o CPF/CNPJ do pagador no header payercpfcnpj.",
            "Envie o DELETE para /api/v1/payer/{tokenPayer} com os headers X-Api-Key e payercpfcnpj."
          ],
          exemplos: {
            curl: `curl -X DELETE {base}/api/v1/payer/67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b \\
  -H "X-Api-Key: SUA_APIKEY" \\
  -H "payercpfcnpj: 12345678909"`,
            csharp: `using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-Key", "SUA_APIKEY");
client.DefaultRequestHeaders.Add("payercpfcnpj", "12345678909");

var tokenPayer = "67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b";
var resposta = await client.DeleteAsync("{base}/api/v1/payer/" + tokenPayer);
var corpo = await resposta.Content.ReadAsStringAsync();
Console.WriteLine(corpo);`,
            powershell: `Invoke-RestMethod -Uri "{base}/api/v1/payer/67f2c5b9-1d4a-4c6e-8f3a-2b9e0c1d4a5b" \`
  -Method Delete \`
  -Headers @{
    "X-Api-Key"    = "SUA_APIKEY"
    "payercpfcnpj" = "12345678909"
  }`
          }
        }
      ]
    },

    {
      id: "conta",
      tipo: "embreve",
      titulo: "Conta",
      menu: "Conta",
      mensagem: "A documentação do módulo Conta será adicionada nas próximas etapas."
    },

    {
      id: "extrato",
      tipo: "embreve",
      titulo: "Extrato",
      menu: "Extrato",
      mensagem: "A documentação do módulo Extrato será adicionada nas próximas etapas."
    }
  ]
};
