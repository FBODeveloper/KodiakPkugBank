# Projeto de integração da ApiKodiakExtrato com a TecnoSpedPlugBank

 - ** toda documentação de integração com o openfinace da pligbank deve ser encontrada aqui:
https://docs.pagamentobancario.com.br/#section/Introducao
 
 em ROTAS DA APLICAÇÃO usaremos apenas PAGADOR, CONTA e OPENFINANCE
 Rotas raiz: (Nos endpoints estará informado como #rotaraiz
    -** Produção: https://api.pagamentobancario.com.br/api/v1/payer
    -** Homologação: https://staging.pagamentobancario.com.br/api/v1/payer
    
  - ** A aplicação usa a base de **Homologação (staging)** por padrão (`appsettings.json`).
     Para usar a base de **Produção**, rodar com `ASPNETCORE_ENVIRONMENT=Production`
     (o `appsettings.Production.json` define `PlugBank:BaseUrl` = produção; o log de startup
     exibe o "PlugBank BaseUrl efetivo" para confirmar).

 - ** PAGADOR: https://docs.pagamentobancario.com.br/#tag/payer
   POST: (Cadastro) #rotaraiz
   PUT:  (Atualização) #rotaraiz
   GET:  (COnsulta pagador) #rotaraiz
   GET:  (Lista) #rotaraiz/list
   DELETE: (Desativar um pagador) #rotaraiz/{tokenPayer}
     ** Atenção tokenPayer é um campo retonado ao cadastradar um pagador (consultar e lista também devolvem esse campo) então
        após receber retorno 200 ou 201 de cadastro de pagador armazenar esse campo junto ao cadastro de pagador para consultas futuras.
   
   DOCUMENTAÇÃO REFERENTE AO PAGADOR: https://atendimento.tecnospeed.com.br/hc/pt-br/articles/35691080001047-Criar-Pagador-e-Conta-para-Extrato-Open-Finance

 - ** CONTA: https://docs.pagamentobancario.com.br/#tag/account
 - ** OPENFINANCE: https://docs.pagamentobancario.com.br/#tag/statementOpenfinance
 
 ##AUTENTICAÇÃO
 - ** na TecnoSpedPlugBank exige 2 campos (entre outros) para autenticação
    cnpjsh: esse campo deve ser buscado de uma variável de ambiente chamada KODIAK_PLUGBANK_SH
	tokensh: esse campo deve ser buscado de uma variável de ambiente chamada KODIAK_PLUGBANK