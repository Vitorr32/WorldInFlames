using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstrucaoController : MonoBehaviour {
    
    public Dropdown investimento;
    public Dropdown nacao;
    public Text descricaoNacao;
    public Text descricaoInvestimento;
    public Text combustivel;
    public Text metal;
    public Text municao;
    public Text maoDeObra;
    public Text inspiracacao;

    public Button construir;
    public Image bandeiraNacaoAtual;
    public Image nivelTecnologicoNaocaoAtual;
    public Image iconeNivelDeInvestimento;

    public List<Nacoes> nacoesAtivas;
    public Nacoes nacaoAtual;

    public Sprite[] niveisTecnologicos = new Sprite[7];
    public Sprite[] niveisDeInvestimento = new Sprite[5];
    
    private int custoCombustivel;
    private int custoMetal;
    private int custoMunicao;
    private int custoMaoDeObra;
    private int custoInspiracao;
    
    public void ApresentarVisualConstrucao(){
        nacao.options.Clear();
        nacoesAtivas = new List<Nacoes>();
        foreach(Nacoes n in GameController.instancia.nacoesDoJogador){
            nacao.options.Add(new Dropdown.OptionData { image = BancoDeDados.instancia.nomesDeNacoes[(int)n.pais]});
            nacoesAtivas.Add(n);
        }
        nacao.value = 0;
        mudarPaisAtual();
    }

    private bool checarRecursos(){
        if (custoCombustivel > GameController.instancia.Combustivel || custoMetal > GameController.instancia.Metal ||
            custoMunicao > GameController.instancia.Municao || custoMaoDeObra > GameController.instancia.MaoDeObra ||
            custoInspiracao > GameController.instancia.Inspiracao){//Checar se o player tem recursos o suficiente
            construir.interactable = false;
            return false;
        }
        construir.interactable = true;
        return true;
    }

    public void ComecarConstrucao(){
        Equipamento novoEquipamento = GameController.instancia.Construcao(investimento.value, nacoesAtivas[nacao.value]);
        //Debug.Log("Equipamento " + novoEquipamento.nome + " em construção");
        GameController.instancia.iniciarConstrucao(novoEquipamento, LinhasDeProducaoController.instancia.linhaSelecionada);
        GameController.instancia.Combustivel -= (int)custoCombustivel;
        GameController.instancia.Metal -= (int)custoMetal;
        GameController.instancia.Municao -= (int)custoMunicao;
        GameController.instancia.MaoDeObra -= (int)custoMaoDeObra;
        GameController.instancia.Inspiracao -= (int)custoInspiracao;
        if (EventosDoHQ.instancia != null) { EventosDoHQ.instancia.atualizarInterface(); }
        
    }

    public void mudarPaisAtual(){
        nacaoAtual = nacoesAtivas[nacao.value];
        nacao.Select();
        nacao.RefreshShownValue();
        mudancaDeInvestimento();
        popularNacaoEscolhida();
    }

    private void popularNacaoEscolhida(){
        bandeiraNacaoAtual.sprite = BancoDeDados.instancia.iconesDeNacoes[(int)nacaoAtual.pais];
        descricaoNacao.text = nacaoAtual.descricao;
        nivelTecnologicoNaocaoAtual.sprite = niveisTecnologicos[nacaoAtual.nivel];
    }

    public void mudancaDeInvestimento(){
        custoCombustivel = (int)(((investimento.value + 1) * 500)*nacaoAtual.modfCombustivel);
        custoMetal = (int)(((investimento.value + 1) * 500)*nacaoAtual.modfMetal);
        custoMunicao = (int)(((investimento.value + 1) * 1000)* nacaoAtual.modfMunicao);
        custoMaoDeObra = (int)(((investimento.value + 1) * 100)*nacaoAtual.modfMaoDeObra);
        custoInspiracao = (int)(((investimento.value + 1) * 10)*nacaoAtual.modfInspiracao);

        checarRecursos();

        combustivel.text = custoCombustivel.ToString();
        metal.text = custoMetal.ToString();
        municao.text = custoMunicao.ToString();
        maoDeObra.text = custoMaoDeObra.ToString();
        inspiracacao.text = custoInspiracao.ToString();
        iconeNivelDeInvestimento.sprite = niveisDeInvestimento[investimento.value];
        
        switch (investimento.value) {            
            case 0:
                descricaoInvestimento.text="Produção de equipamentos antigos muitas vezes partiam do desespero e falta de recursos, esses equipamentos são normalmente achados em antigos arsenais, datando até o século passado, normalmente dado a um exercito feito para uma milicia para fazer resistência a ocupação inimigas";
                break;
            case 1:
                descricaoInvestimento.text="Criação de equipamentos antiquados tenta focar em tapar os buracos que baixas ou capturas de arsenais deixaram no equipamentos do exercito, esse equipamento e tirado de arsenais da Primeira Guerra Mundial e dificilmente iriam resistir muito tempo a um inimigo bem equipado";
                break;
            case 2:
                descricaoInvestimento.text="Produção de equipamentos regulares e aqueles que ja se provaram confiaveis, que sempre funcionam e por isso são usado em grande escala por todo o exercito, normalmente esses equipamentos tem no maximo 5 anos desde a sua produção original";
                break;
            case 3:
                descricaoInvestimento.text= "Equipamentos mordenos são os mais recentes produzidos pelo exercito que podem se considerar confiaveis e seguros para uso, unidades que usam equipamentos podem se considerar da elite do exercito, esses equipamentos tem no maximo 2 anos de idade";
                break;
            case 4:
                descricaoInvestimento.text = "Equipamentos experimentais são o melhor que um exercito pode oferecer, contando com as tencologias mais atuais possiveis, apesar de ainda não serem considerados confiaveis o suficiente sua projeção de poder deve surpreender todo e qualquer exercito inimigo ainda não acostumado com a tecnologia";
                break;
        }   
    }    
}
