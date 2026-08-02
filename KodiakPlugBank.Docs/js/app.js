(function () {
  "use strict";

  var docs = window.KODIAK_DOCS;
  var conteudo = document.getElementById("conteudo");
  var menu = document.getElementById("menu");
  var busca = document.getElementById("buscaMenu");
  var seletor = document.getElementById("seletorAmbiente");
  var btnTema = document.getElementById("btnTema");
  var linkSwagger = document.getElementById("linkSwagger");

  var CHAVE_TEMA = "kodiak-docs-tema";
  var CHAVE_AMBIENTE = "kodiak-docs-ambiente";
  var idContador = 0;

  var CORES_METODO = {
    POST: "success",
    GET: "primary",
    PUT: "warning",
    DELETE: "danger",
    PATCH: "secondary"
  };

  var armazenamento = (function () {
    try {
      var teste = "kodiak-docs-teste";
      window.localStorage.setItem(teste, "1");
      window.localStorage.removeItem(teste);
      return window.localStorage;
    } catch (e) {
      return { getItem: function () { return null; }, setItem: function () {} };
    }
  })();

  function esc(html) {
    var div = document.createElement("div");
    div.textContent = html;
    return div.innerHTML;
  }

  function badgeMetodo(metodo) {
    var cor = CORES_METODO[metodo] || "secondary";
    var extra = cor === "warning" ? " text-dark" : "";
    return '<span class="badge-metodo badge text-bg-' + cor + extra + '">' + esc(metodo) + "</span>";
  }

  function acharSecao(id) {
    return docs.secoes.find(function (s) { return s.id === id; });
  }

  function acharEndpoint(secao, id) {
    return secao.endpoints.find(function (e) { return e.id === id; });
  }

  /* ------------------------- Ambiente ------------------------- */

  function ambienteAtual() {
    var id = armazenamento.getItem(CHAVE_AMBIENTE);
    return docs.ambientes.find(function (a) { return a.id === id; }) || docs.ambientes[0];
  }

  function aplicarBase(texto) {
    if (!texto) return "";
    return texto.split("{base}").join(ambienteAtual().base);
  }

  function atualizarSwagger() {
    var base = ambienteAtual().base;
    var ehHomologacao = ambienteAtual().id === "homologacao";
    linkSwagger.href = base + "/swagger";
    linkSwagger.classList.toggle("d-sm-inline-flex", ehHomologacao);
    linkSwagger.classList.toggle("d-none", !ehHomologacao);
  }

  function preencherAmbientes() {
    docs.ambientes.forEach(function (a) {
      var op = document.createElement("option");
      op.value = a.id;
      op.textContent = a.nome;
      seletor.appendChild(op);
    });
    seletor.value = ambienteAtual().id;
    seletor.addEventListener("change", function () {
      armazenamento.setItem(CHAVE_AMBIENTE, seletor.value);
      atualizarSwagger();
      renderRota();
    });
    atualizarSwagger();
  }

  /* ------------------------- Tema ------------------------- */

  function temaAtual() {
    return document.documentElement.getAttribute("data-bs-theme") || "dark";
  }

  function aplicarTema(tema) {
    document.documentElement.setAttribute("data-bs-theme", tema);
    armazenamento.setItem(CHAVE_TEMA, tema);
    var icone = btnTema.querySelector("i");
    if (icone) {
      icone.className = "bi " + (tema === "dark" ? "bi-sun" : "bi-moon");
    }
    btnTema.title = tema === "dark" ? "Ativar tema claro" : "Ativar tema escuro";
    btnTema.setAttribute("aria-label", btnTema.title);
  }

  function iniciarTema() {
    var salvo = armazenamento.getItem(CHAVE_TEMA);
    aplicarTema(salvo || "dark");
    btnTema.addEventListener("click", function () {
      aplicarTema(temaAtual() === "dark" ? "light" : "dark");
    });
  }

  /* ------------------------- Copiar ------------------------- */

  function copiarTexto(texto) {
    if (navigator.clipboard && window.isSecureContext) {
      return navigator.clipboard.writeText(texto);
    }
    return new Promise(function (resolver, rejeitar) {
      var ta = document.createElement("textarea");
      ta.value = texto;
      ta.style.position = "fixed";
      ta.style.opacity = "0";
      document.body.appendChild(ta);
      ta.select();
      try {
        document.execCommand("copy");
        resolver();
      } catch (e) {
        rejeitar(e);
      } finally {
        document.body.removeChild(ta);
      }
    });
  }

  function iniciarCopiar() {
    document.addEventListener("click", function (e) {
      var btn = e.target.closest ? e.target.closest(".btn-copiar") : null;
      if (!btn) return;
      var bloco = btn.closest(".bloco-codigo");
      if (!bloco) return;
      var pre = bloco.querySelector("pre");
      if (!pre) return;
      var original = btn.innerHTML;
      copiarTexto(pre.textContent).then(function () {
        btn.innerHTML = '<i class="bi bi-check2"></i> Copiado!';
        setTimeout(function () { btn.innerHTML = original; }, 1600);
      }).catch(function () {
        btn.innerHTML = '<i class="bi bi-x-lg"></i> Erro';
        setTimeout(function () { btn.innerHTML = original; }, 1600);
      });
    });
  }

  /* ------------------------- Blocos de código ------------------------- */

  function blocoCodigoSimples(codigo) {
    if (!codigo) return "";
    return '<div class="bloco-codigo">' +
      '<button type="button" class="btn-copiar"><i class="bi bi-clipboard"></i> Copiar</button>' +
      "<pre><code>" + esc(codigo) + "</code></pre>" +
      "</div>";
  }

  function blocoCodigo(titulo, icone, codigo) {
    if (!codigo) return "";
    return '<div class="card"><div class="card-body">' +
      '<h5 class="mb-3"><i class="bi ' + icone + ' me-2 text-info"></i>' + esc(titulo) + "</h5>" +
      blocoCodigoSimples(codigo) +
      "</div></div>";
  }

  /* ------------------------- Tabelas ------------------------- */

  function tabelaHeaders(headers) {
    if (!headers || headers.length === 0) return "";
    var linhas = headers.map(function (h) {
      var obrig = h.obrigatorio
        ? '<span class="obrigatorio">Sim</span>'
        : '<span class="text-secondary">Não</span>';
      return "<tr><td><code>" + esc(h.nome) + "</code></td><td>" + obrig + "</td><td>" + esc(h.descricao) + "</td></tr>";
    }).join("");
    return `
      <div class="card">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-headset me-2 text-info"></i>Headers da requisição</h5>
          <table class="table align-middle mb-0">
            <thead><tr><th>Header</th><th>Obrigatório</th><th>Descrição</th></tr></thead>
            <tbody>${linhas}</tbody>
          </table>
        </div>
      </div>`;
  }

  function tabelaCampos(campos) {
    if (!campos || campos.length === 0) return "";
    var linhas = campos.map(function (c) {
      var obrig = c.obrigatorio
        ? '<span class="obrigatorio">Sim</span>'
        : '<span class="text-secondary">Não</span>';
      return "<tr>" +
        "<td><code>" + esc(c.nome) + "</code></td>" +
        '<td><span class="tipo-campo text-info">' + esc(c.tipo) + "</span></td>" +
        "<td>" + obrig + "</td>" +
        "<td>" + esc(c.descricao) + "</td>" +
        "</tr>";
    }).join("");
    return `
      <div class="card">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-json me-2 text-info"></i>Parâmetros do corpo</h5>
          <table class="table align-middle mb-0">
            <thead><tr><th>Campo</th><th>Tipo</th><th>Obrigatório</th><th>Descrição</th></tr></thead>
            <tbody>${linhas}</tbody>
          </table>
        </div>
      </div>`;
  }

  function tabelaCamposResposta(campos) {
    if (!campos || campos.length === 0) return "";
    var linhas = campos.map(function (c) {
      return "<tr>" +
        "<td><code>" + esc(c.nome) + "</code></td>" +
        '<td><span class="tipo-campo text-info">' + esc(c.tipo) + "</span></td>" +
        "<td>" + esc(c.descricao) + "</td>" +
        "</tr>";
    }).join("");
    return `
      <div class="card">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-table me-2 text-info"></i>Campos da resposta</h5>
          <table class="table align-middle mb-0">
            <thead><tr><th>Campo</th><th>Tipo</th><th>Descrição</th></tr></thead>
            <tbody>${linhas}</tbody>
          </table>
        </div>
      </div>`;
  }

  function tabelaErros(erros) {
    if (!erros || erros.length === 0) return "";
    var linhas = erros.map(function (e) {
      return "<tr>" +
        '<td class="status-badge text-warning">' + esc(e.status) + "</td>" +
        "<td><code>" + esc(e.mensagem) + "</code></td>" +
        "<td>" + esc(e.descricao) + "</td>" +
        "</tr>";
    }).join("");
    return `
      <div class="card tabela-erros">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-exclamation-octagon me-2 text-info"></i>Erros esperados</h5>
          <table class="table align-middle mb-0">
            <thead><tr><th>Status</th><th>Mensagem</th><th>Descrição</th></tr></thead>
            <tbody>${linhas}</tbody>
          </table>
        </div>
      </div>`;
  }

  /* ------------------------- Abas de exemplos ------------------------- */

  function abasExemplos(exemplos) {
    if (!exemplos) return "";
    var idiomas = [
      { chave: "curl", rotulo: "cURL" },
      { chave: "csharp", rotulo: "C#" },
      { chave: "powershell", rotulo: "PowerShell" },
      { chave: "python", rotulo: "Python" },
      { chave: "node", rotulo: "Node.js" },
      { chave: "react", rotulo: "React" },
      { chave: "nextjs", rotulo: "Next.js" }
    ];
    var presentes = idiomas.filter(function (i) { return exemplos[i.chave]; });
    if (presentes.length === 0) return "";

    var id = "exemplos-" + (++idContador);
    var navs = presentes.map(function (p, i) {
      return '<button class="nav-link' + (i === 0 ? " active" : "") + '" data-bs-toggle="tab" data-bs-target="#' + id + "-" + p.chave + '" type="button" role="tab">' + esc(p.rotulo) + "</button>";
    }).join("");
    var panes = presentes.map(function (p, i) {
      var codigo = aplicarBase(exemplos[p.chave]);
      return '<div class="tab-pane fade' + (i === 0 ? " show active" : "") + '" id="' + id + "-" + p.chave + '" role="tabpanel">' +
        blocoCodigoSimples(codigo) +
        "</div>";
    }).join("");

    return `
      <div class="card">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-terminal me-2 text-info"></i>Exemplos de chamada</h5>
          <ul class="nav nav-tabs tabs-exemplos" role="tablist">${navs}</ul>
          <div class="tab-content">${panes}</div>
        </div>
      </div>`;
  }

  /* ------------------------- Renderização ------------------------- */

  function passosComoUsar(passos) {
    if (!passos || passos.length === 0) return "";
    var itens = passos.map(function (p, i) {
      return '<div class="passo"><div class="num">' + (i + 1) + '</div><div>' + esc(p) + "</div></div>";
    }).join("");
    return `
      <div class="card">
        <div class="card-body">
          <h5 class="mb-3"><i class="bi bi-lightbulb me-2 text-info"></i>Como utilizar</h5>
          ${itens}
        </div>
      </div>`;
  }

  function renderEndpoint(secao, endpoint) {
    var rotaLinha =
      '<div class="rota-codigo d-flex align-items-center gap-2">' +
      badgeMetodo(endpoint.metodo) +
      "<span>" + esc(endpoint.rota) + "</span></div>";

    var html =
      '<div class="pagina-titulo">' +
      "<h1>" + badgeMetodo(endpoint.metodo) + " " + esc(endpoint.rota) + "</h1>" +
      "<p>" + esc(endpoint.resumo) + "</p>" +
      "</div>" +
      '<div class="mb-3">' + rotaLinha + "</div>" +

      '<div class="card"><div class="card-body">' +
      '<h5 class="mb-3"><i class="bi bi-chat-square-text me-2 text-info"></i>O que este endpoint faz</h5>' +
      "<p class=\"mb-0\">" + esc(endpoint.oqueFaz) + "</p>" +
      "</div></div>" +

      tabelaHeaders(endpoint.headers) +
      tabelaCampos(endpoint.campos) +
      blocoCodigo("Corpo da requisição", "bi-box-arrow-in-up", aplicarBase(endpoint.bodyExemplo)) +

      '<div class="card"><div class="card-body">' +
      '<h5 class="mb-3"><i class="bi bi-box-arrow-in-down me-2 text-info"></i>Resposta</h5>' +
      '<div class="mb-2"><span class="chip chip-obrig">' + esc(endpoint.respostaStatus) + "</span></div>" +
      "<p>" + esc(endpoint.respostaDescricao) + "</p>" +
      blocoCodigoSimples(aplicarBase(endpoint.respostaExemplo)) +
      "</div></div>" +

      tabelaCamposResposta(endpoint.respostaCampos) +
      tabelaErros(endpoint.erros) +
      passosComoUsar(endpoint.comoUsar) +
      abasExemplos(endpoint.exemplos);

    conteudo.innerHTML = html;
    window.scrollTo({ top: 0 });
  }

  function renderGrupo(secao) {
    var html =
      '<div class="pagina-titulo">' +
      "<h1>" + esc(secao.titulo) + "</h1>" +
      "<p>" + esc(secao.descricao || "") + "</p>" +
      "</div>" +
      '<div class="card"><div class="card-body">' +
      "<h5 class=\"mb-3\"><i class=\"bi bi-list-ul me-2 text-info\"></i>Endpoints do módulo</h5>" +
      "<ul class=\"list-unstyled indice-endpoints mb-0\">" +
      secao.endpoints.map(function (e) {
        return '<li class="mb-3">' +
          badgeMetodo(e.metodo) +
          ' <a href="#/' + secao.id + "/" + e.id + '"><code>' + esc(e.rota) + "</code></a>" +
          '<p class="mb-0 mt-1 text-secondary">' + esc(e.resumo) + "</p>" +
          "</li>";
      }).join("") +
      "</ul>" +
      "</div></div>";
    conteudo.innerHTML = html;
    window.scrollTo({ top: 0 });
  }

  function renderHtml(secao) {
    conteudo.innerHTML =
      '<div class="pagina-titulo">' +
      "<h1>" + esc(secao.titulo) + "</h1>" +
      "</div>" + aplicarBase(secao.html);
    window.scrollTo({ top: 0 });
  }

  function renderEmBreve(secao) {
    conteudo.innerHTML =
      '<div class="pagina-titulo">' +
      "<h1>" + esc(secao.titulo) + "</h1>" +
      "</div>" +
      '<div class="embreve">' +
      '<i class="bi bi-hammer"></i>' +
      "<h4>" + esc(secao.titulo) + "</h4>" +
      "<p class=\"text-secondary mb-0\">" + esc(secao.mensagem) + "</p>" +
      "</div>";
  }

  /* ------------------------- Menu ------------------------- */

  function marcarAtivo(rota) {
    Array.prototype.forEach.call(menu.querySelectorAll(".nav-link"), function (link) {
      var ativo = link.dataset.rota === rota;
      link.classList.toggle("ativo", ativo);
    });

    var toggle = menu.querySelector(".sidebar-submenu-toggle");
    if (toggle) {
      toggle.classList.toggle("ativo", rota === "pagador" || rota.indexOf("pagador/") === 0);
    }
  }

  function expandirSubmenu(secaoId) {
    if (secaoId !== "pagador") return;
    var sub = document.getElementById("sub-pagador");
    if (sub && window.bootstrap && !sub.classList.contains("show")) {
      bootstrap.Collapse.getOrCreateInstance(sub).show();
    }
  }

  function filtrarMenu() {
    var q = (busca.value || "").trim().toLowerCase();
    var temQ = q.length > 0;

    Array.prototype.forEach.call(menu.querySelectorAll(".nav-link:not(.sub)"), function (link) {
      var match = !temQ || link.textContent.toLowerCase().indexOf(q) !== -1;
      link.classList.toggle("d-none", !match);
    });

    Array.prototype.forEach.call(menu.querySelectorAll(".nav-link.sub"), function (link) {
      var match = !temQ || link.textContent.toLowerCase().indexOf(q) !== -1;
      link.classList.toggle("d-none", !match);
    });

    var toggle = menu.querySelector(".sidebar-submenu-toggle");
    if (toggle) {
      var texto = toggle.textContent.toLowerCase();
      var temSubVisivel = Array.prototype.some.call(menu.querySelectorAll(".nav-link.sub"), function (l) {
        return !l.classList.contains("d-none");
      });
      var match = !temQ || texto.indexOf(q) !== -1 || temSubVisivel;
      toggle.classList.toggle("d-none", !match);

      if (temQ && temSubVisivel) {
        var sub = document.getElementById("sub-pagador");
        if (sub && window.bootstrap) {
          bootstrap.Collapse.getOrCreateInstance(sub).show();
        }
      }
    }

    Array.prototype.forEach.call(menu.querySelectorAll(".sidebar-grupo"), function (g) {
      var proximo = g.nextElementSibling;
      var match = !temQ || (proximo && !proximo.classList.contains("d-none"));
      g.classList.toggle("d-none", !match);
    });
  }

  /* ------------------------- Roteamento ------------------------- */

  function renderRota() {
    var hash = window.location.hash || "#/visao-geral";
    var partes = hash.replace(/^#\//, "").split("/");
    var secaoId = partes[0];
    var endpointId = partes[1] || null;

    var secao = acharSecao(secaoId);
    if (!secao) secao = acharSecao("visao-geral");

    expandirSubmenu(secao.id);

    if (secao.tipo === "html") {
      renderHtml(secao);
      marcarAtivo(secao.id);
    } else if (secao.tipo === "grupo") {
      var endpoint = endpointId ? acharEndpoint(secao, endpointId) : null;
      if (endpoint) {
        renderEndpoint(secao, endpoint);
        marcarAtivo(secao.id + "/" + endpointId);
      } else {
        renderGrupo(secao);
        marcarAtivo(secao.id);
      }
    } else {
      renderEmBreve(secao);
      marcarAtivo(secao.id);
    }
  }

  function navegar() {
    renderRota();

    var offcanvas = document.getElementById("sidebar");
    if (offcanvas && offcanvas.classList.contains("show")) {
      var instancia = bootstrap.Offcanvas.getInstance(offcanvas);
      if (instancia) instancia.hide();
    }
  }

  window.addEventListener("hashchange", navegar);
  document.addEventListener("DOMContentLoaded", function () {
    if (!window.bootstrap) {
      console.error("Bootstrap não carregado. Verifique a conexão com o CDN.");
    }
    preencherAmbientes();
    iniciarTema();
    iniciarCopiar();
    if (busca) busca.addEventListener("input", filtrarMenu);
    navegar();
  });
})();
