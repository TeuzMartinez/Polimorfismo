// Parte 1: Polimorfismo e Métodos Virtuais

// Exercício 1.1: Processador de Documentos
public abstract class Documento
{
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public DateTime DataCriacao { get; set; }

    public virtual void Imprimir()
    {
        Console.WriteLine($"Titulo: {Titulo}, Autor: {Autor}, Data: {DataCriacao}");
    }

    public virtual string ConteudoFormatado() => $"Documento: {Titulo}";
}

public class DocumentoTexto : Documento
{
    public string Conteudo { get; set; }
    public override void Imprimir() => Console.WriteLine(ConteudoFormatado());
    public override string ConteudoFormatado() => $"Texto: {Conteudo}";
    public int ContarPalavras() => Conteudo?.Split(' ').Length ?? 0;
}

public class DocumentoHTML : Documento
{
    public string Html { get; set; }
    public string Estilo { get; set; }
    public override void Imprimir() => Console.WriteLine(ConteudoFormatado());
    public override string ConteudoFormatado() => $"<style>{Estilo}</style>{Html}";
    public void AdicionarEstilo(string css) => Estilo = css;
}

public class DocumentoPDF : Documento
{
    public string ConteudoPdf { get; set; }
    public string MarcaDagua { get; set; }
    public override void Imprimir() => Console.WriteLine(ConteudoFormatado());
    public override string ConteudoFormatado() => $"PDF: {ConteudoPdf}\nMarca: {MarcaDagua}";
    public void AdicionarMarcaDagua(string texto) => MarcaDagua = texto;
}

public class ProcessadorDocumentos
{
    public void ProcessarLote(List<Documento> documentos)
    {
        foreach (var doc in documentos)
        {
            doc.Imprimir();
            Console.WriteLine("----------------------");
        }
    }
}

// Exercício 1.2: Sistema de Notificações
public abstract class Notificacao
{
    public string Destinatario { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataEnvio { get; set; }

    public virtual void Enviar() => Console.WriteLine("Enviando notificação...");
    public virtual string FormatarMensagem() => Mensagem;
}

public class NotificacaoEmail : Notificacao
{
    public override void Enviar() => Console.WriteLine($"Enviando e-mail para {Destinatario}: {Mensagem}");
    public override string FormatarMensagem() => $"[Email] {Mensagem}";
}

public class NotificacaoSMS : Notificacao
{
    public override void Enviar() => Console.WriteLine($"Enviando SMS para {Destinatario}: {Mensagem}");
    public override string FormatarMensagem() => $"[SMS] {Mensagem}";
}

public class NotificacaoPush : Notificacao
{
    public override void Enviar() => Console.WriteLine($"Enviando Push para {Destinatario}: {Mensagem}");
    public override string FormatarMensagem() => $"[Push] {Mensagem}";
}

// Exercício 1.3: Ocultação vs Sobrescrita
public class RegistroBase
{
    public virtual void Salvar() => Console.WriteLine("Salvar no RegistroBase");
}

public class RegistroSobrescrito : RegistroBase
{
    public override void Salvar() => Console.WriteLine("Salvar no RegistroSobrescrito");
}

public class RegistroOculto : RegistroBase
{
    public new void Salvar() => Console.WriteLine("Salvar no RegistroOculto");
}

// Parte 2: Interfaces e Implementação

public interface IArmazenamento
{
    bool Salvar(string nome, byte[] dados);
    byte[] Carregar(string nome);
    bool Excluir(string nome);
    List<string> ListarArquivos();
}

public interface IRastreavel
{
    void RegistrarOperacao(string operacao, string arquivo);
    List<string> ObterHistoricoOperacoes();
}

public class ArmazenamentoLocal : IArmazenamento
{
    private Dictionary<string, byte[]> _arquivos = new();

    public bool Salvar(string nome, byte[] dados)
    {
        _arquivos[nome] = dados;
        return true;
    }
    public byte[] Carregar(string nome) => _arquivos.ContainsKey(nome) ? _arquivos[nome] : null;
    public bool Excluir(string nome) => _arquivos.Remove(nome);
    public List<string> ListarArquivos() => _arquivos.Keys.ToList();
}

public class ArmazenamentoNuvem : IArmazenamento, IRastreavel
{
    private Dictionary<string, byte[]> _arquivos = new();
    private List<string> _historico = new();

    public bool Salvar(string nome, byte[] dados)
    {
        _arquivos[nome] = dados;
        RegistrarOperacao("Salvar", nome);
        return true;
    }
    public byte[] Carregar(string nome)
    {
        RegistrarOperacao("Carregar", nome);
        return _arquivos.ContainsKey(nome) ? _arquivos[nome] : null;
    }
    public bool Excluir(string nome)
    {
        var result = _arquivos.Remove(nome);
        RegistrarOperacao("Excluir", nome);
        return result;
    }
    public List<string> ListarArquivos() => _arquivos.Keys.ToList();

    public void RegistrarOperacao(string operacao, string arquivo) => _historico.Add($"{operacao} - {arquivo}");
    public List<string> ObterHistoricoOperacoes() => _historico;
}

public class GerenciadorArquivos
{
    private readonly IArmazenamento _armazenamento;
    public GerenciadorArquivos(IArmazenamento armazenamento) => _armazenamento = armazenamento;
    public void SalvarArquivo(string nome, byte[] dados) => _armazenamento.Salvar(nome, dados);
}

// Exercício 2.2: Interfaces Explícitas
public interface IServicoEmail { void Enviar(string destinatario, string assunto, string corpo); }
public interface IServicoSMS { void Enviar(string numero, string mensagem); }

public class ServicoNotificacao : IServicoEmail, IServicoSMS
{
    void IServicoEmail.Enviar(string destinatario, string assunto, string corpo)
        => Console.WriteLine($"Email para {destinatario}: {assunto} - {corpo}");

    void IServicoSMS.Enviar(string numero, string mensagem)
        => Console.WriteLine($"SMS para {numero}: {mensagem}");
}

// Parte 3: IDisposable
public class GerenciadorConexao : IDisposable
{
    private bool _disposed = false;
    public GerenciadorConexao() => Console.WriteLine("Conexão iniciada");

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine("Dispose chamado");
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~GerenciadorConexao() => Console.WriteLine("Finalizador chamado");
}

public class RecursoBase : IDisposable
{
    public virtual void Dispose() => Console.WriteLine("Liberando RecursoBase");
}

public class RecursoArquivo : RecursoBase
{
    public override void Dispose()
    {
        Console.WriteLine("Liberando RecursoArquivo");
        base.Dispose();
    }
}

public class RecursoBancoDados : RecursoBase
{
    public override void Dispose()
    {
        Console.WriteLine("Liberando RecursoBancoDados");
        base.Dispose();
    }
}

// Parte 4: Sistema de E-commerce
public abstract class Produto
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public virtual decimal CalcularPrecoFinal() => Preco;
    public virtual void ExibirDetalhes() => Console.WriteLine($"Produto: {Nome} - Preço: {Preco}");
}

public interface IEntregavel { void Entregar(); }
public interface ITributavel { decimal CalcularImposto(); }
public interface IReembolsavel { void Reembolsar(); }

public class ProdutoFisico : Produto, IEntregavel, ITributavel
{
    public decimal Peso { get; set; }
    public override decimal CalcularPrecoFinal() => Preco + CalcularImposto();
    public decimal CalcularImposto() => Preco * 0.1m;
    public void Entregar() => Console.WriteLine("Entregando produto físico...");
}

public class ProdutoDigital : Produto, IReembolsavel
{
    public string LinkDownload { get; set; }
    public override decimal CalcularPrecoFinal() => Preco;
    public void Reembolsar() => Console.WriteLine("Reembolso do produto digital...");
}

public class Servico : Produto
{
    public int DuracaoDias { get; set; }
    public override decimal CalcularPrecoFinal() => Preco;
}

public class CarrinhoCompra
{
    private List<Produto> _produtos = new();
    public void AdicionarProduto(Produto produto) => _produtos.Add(produto);
    public decimal CalcularTotal() => _produtos.Sum(p => p.CalcularPrecoFinal());
}

public interface IProcessadorPagamento { void ProcessarPagamento(decimal valor); }
public class ProcessadorCartaoCredito : IProcessadorPagamento
{
    public void ProcessarPagamento(decimal valor) => Console.WriteLine($"Pagamento no crédito: {valor:C}");
}
public class ProcessadorBoleto : IProcessadorPagamento
{
    public void ProcessarPagamento(decimal valor) => Console.WriteLine($"Gerando boleto no valor de: {valor:C}");
}
public class ProcessadorPix : IProcessadorPagamento
{
    public void ProcessarPagamento(decimal valor) => Console.WriteLine($"Pagamento via PIX: {valor:C}");
}