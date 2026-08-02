# Projeto Kodiak PlugBank
** objetivo: consultar extratos bancários utilizando integração com a api do open finance da TecnoSpedPlugBank (arquivo IntegacaoPlugBankTecnoSped.md)

** Caracteristica:
 -* Esse é o projeto que o sistema Kodiak ERP vai utilizar, todo Kodiak ERP vai se conectar a uma única a api, que é esse projeto, e esse Projeto
 fará uso da comunicação com a api da TecnoSpedPlugBank para obter os extratos.
 
 ** Fluxo de operação:
  KodiakERP aciona essa api
  -- Essa api se conecta à api da TecnoSpedPlugBank
     -- Api da TecnoSpedPlugBank faz a comunicação com o OpenFinance e obtem o extrato da empresa.
	   -- KodiakERP obtem o retorno da da api TecnoSpedPlugBank e exibe no ERP.
	  
	  
** Funções necessárias:
 -** Cadastro de Pagador:
     são as empresas que farão uso da api tanto no KodiakERP quanto no TecnoSpedPlugBank;
	 Os campos necessários estão em: https://docs.pagamentobancario.com.br/#tag/payer/operation/createPayer
	 O campo ChaveKodiakExtrato (até 1.000 caracteres) é persistido no banco/entidade, porém NÃO é mais utilizado
	 para validação: a autenticação é feita exclusivamente pela apikey fixa (tabela apikey_fixa).
	 Rotas de pagador (espelham a PlugBank): POST (cadastro), GET (consulta por header payercpfcnpj), PUT (atualização),
	 GET /list (lista) e DELETE /{tokenPayer} (desativa). O campo tokenPayer (token) retornado no cadastro é armazenado
	 na coluna pagador.token para uso nos demais cadastros (conta) e consulta de extrato.
	 Apesar das contas poderem serem associadas ao pagador junto a criação do pagador vamos, nesse projeto, fazer isso, sempre posteriormente usando os 
	 endpoints de Conta.
-** Cadastro de Conta:
    são as contas bancárias das empresas (Pagador) que terão permissão de acesso aos extratos bancários.
	Os campos necessários estão em: https://docs.pagamentobancario.com.br/#tag/account/operation/createAccount
	Sempre adicionar um campo a mais tanto em banco de dados com em entidades chamado: idContaBancariaKodiak integer.
-** Extrato Bancário:
    Criar as necessiadades conforme documentação: https://docs.pagamentobancario.com.br/#tag/statementOpenfinance
	
##BANCKEND
 -- .net10 com dapper e npgsql, não utilizar entity, não utilizar nada de terceiros sem extrema necessidade.
 -- Autenticação, exclusive, via apikey.
 -- Separar o projeto em core, usercase, infra e api.
 -- Usar arquitetura limpa e cleancode, sempre quando necessário, é um projeto pequeno então otimizar.
 -- Sempre que criar um endpoint e esse endpoit for utilizar os endpoints da api da TecnoSpedPlugBank copiar a forma de requisição dos campos,
    ou seja, se um campo está no header crie esse endpoint solicitando o campo em header, se está em body em body para adequação e padronização.
 
 
 ##BANCO DE DADOS
 -- Banco de dados: Postgres18 rodando em localhost porta 5432 user: postgres  senha: 123!asd
 
##DOCUMENTAÇÃO DE USO (SWAGGER)
 -- Disponível apenas em ambiente de desenvolvimento.
 -- UI interativa: http://localhost:<porta>/swagger
 -- Documento JSON: http://localhost:<porta>/swagger/v1/swagger.json
 -- Para testar os endpoints autenticados, clicar em "Authorize" na UI do Swagger e
    informar a apikey fixa do KodiakERP (header X-Api-Key).
 
 ##FRONTENT 
 -- Ainda em definição