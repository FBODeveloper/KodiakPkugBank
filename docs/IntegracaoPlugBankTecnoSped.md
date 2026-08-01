# Projeto de integração da ApiKodiakExtrato com a TecnoSpedPlugBank

 - ** toda documentação de integração com o openfinace da pligbank deve ser encontrada aqui:
https://docs.pagamentobancario.com.br/#section/Introducao
 
 em ROTAS DA APLICAÇÃO usaremos apenas PAGADOR, CONTA e OPENFINANCE
 
 - ** PAGADOR: https://docs.pagamentobancario.com.br/#tag/payer
 - ** CONTA: https://docs.pagamentobancario.com.br/#tag/account
 - ** OPENFINANCE: https://docs.pagamentobancario.com.br/#tag/statementOpenfinance
 
 ##AUTENTICAÇÃO
 - ** na TecnoSpedPlugBank exige 2 campos (entre outros) para autenticação
    cnpjsh: esse campo deve ser buscado de uma variável de ambiente chamada KODIAK_PLUGBANK_SH
	tokensh: esse campo deve ser buscado de uma variável de ambiente chamada KODIAK_PLUGBANK