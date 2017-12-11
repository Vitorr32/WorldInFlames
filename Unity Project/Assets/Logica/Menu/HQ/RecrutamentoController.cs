using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecrutamentoController : MonoBehaviour {
    
    public Dropdown investimento;
    public Dropdown nacao;
    public Text descricaoNacao;
    public Text descricaoInvestimento;
    public Text combustivel;
    public Text metal;
    public Text municao;
    public Text maoDeObra;
    public Text inspiracacao;

    public Button recrutar;
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
    
    public void ApresentarVisualRecrutamento(){
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
            recrutar.interactable = false;
            return false;
        }
        recrutar.interactable = true;
        return true;
    }

    public void ComecarRecrutamento(){
        Unidade novaUnidade = GameController.instancia.Recrutamento(investimento.value, nacoesAtivas[nacao.value]);
        GameController.instancia.iniciarRecrutamento(novaUnidade, LinhasDeProducaoController.instancia.linhaSelecionada);
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
                descricaoInvestimento.text="Tentar recrutar com investimento pobre muitas vezes resultam em unidades mal treinadas ou precariamente equipadas, ainda 'verdes' em com pouquissima experiencia em batalha, use isso apenas se tiver com poucos recursos";
                break;
            case 1:
                descricaoInvestimento.text="Um recrutamento com investimento basico resulta em unidades minimamente treinadas e razoalvemente equipadas, podendo servir como reservas de um exercito";
                break;
            case 2:
                descricaoInvestimento.text="O recrutamento regular resulta num processo de treinamento de meses das tropas antes delas estarem dispostas ao campo de batalha, elas também contam com um equipamento regularizado e que na maioria das vezes ainda não está no padrão mais atual";
                break;
            case 3:
                descricaoInvestimento.text= "Ao enforçar um recrutamento de alto padrão o exercito faz uma profunda seleção entre suas colunas por oficiais e soldados de alta qualidade, muitas vezes para criar unidades de elite e altamente fieis ao país";
                break;
            case 4:
                descricaoInvestimento.text = "No final, não existe recrutas melhores que aqueles provados em batalha, um recrutamento de elites procurar o melhor que o país pode oferecer tanto em soldados rasos até os oficiais do batalhão em todos os fronts de batalha, criando poderosas unidades que são usadas como simbolo de poder e força da nação";
                break;
        }   
    }    
}
