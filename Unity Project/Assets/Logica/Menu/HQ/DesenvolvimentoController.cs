using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DesenvolvimentoController : MonoBehaviour {
    public Dropdown nacao;
    public Text descricaoProximoNivel;
    public Text descricaoNivelAtual;
    public Text combustivel;
    public Text metal;
    public Text municao;
    public Text maoDeObra;
    public Text inspiracacao;

    public Button desenvolver;

    public Image anoAtual;
    public Image proximoAno;

    public List<Nacoes> nacoesAtivas;
    public Nacoes nacaoAtual;
    public Sprite[] anosDeDesenvolvimento = new Sprite[7];
    
    private float custoCombustivel;
    private float custoMetal;
    private float custoMunicao;
    private float custoMaoDeObra;
    private float custoInspiracao;
    
    public void mudarPaisAtual(){
        nacaoAtual = nacoesAtivas[nacao.value];
        nacao.Select();
        nacao.RefreshShownValue();
        PopularDesenvolvimentoHolder();
        mostrarCustos();
    }

    public void ApresentarVisualDesenvolvimento(){
        nacao.options.Clear();
        nacoesAtivas = new List<Nacoes>();
        foreach(Nacoes n in GameController.instancia.nacoesDoJogador){
            if (n.nivel != n.nivelLimite && !n.emDesenvolvimento){
                nacao.options.Add(new Dropdown.OptionData { image = BancoDeDados.instancia.nomesDeNacoes[(int)n.pais]});
                nacoesAtivas.Add(n);
            }
        }
        nacaoAtual = nacoesAtivas[0];
        nacao.value = 0;

        PopularDesenvolvimentoHolder();

        mudarPaisAtual();
        checarRecursos();
    }

    private void PopularDesenvolvimentoHolder(){
        descricaoNivelAtual.text = nacaoAtual.descricoesTecnologicas[nacaoAtual.nivel];
        descricaoProximoNivel.text = nacaoAtual.descricoesTecnologicas[nacaoAtual.nivel + 1];
        anoAtual.sprite = anosDeDesenvolvimento[nacaoAtual.nivel];
        proximoAno.sprite = anosDeDesenvolvimento[nacaoAtual.nivel + 1];
    }

    private bool checarRecursos(){
        if (custoCombustivel > GameController.instancia.Combustivel || custoMetal > GameController.instancia.Metal ||
            custoMunicao > GameController.instancia.Municao || custoMaoDeObra > GameController.instancia.MaoDeObra ||
            custoInspiracao > GameController.instancia.Inspiracao){//Checar se o player tem recursos o suficiente
            desenvolver.interactable = false;
            return false;
        }
        desenvolver.interactable = true;
        return true;
    }

    public void DesenvolverNacao(){
        GameController.instancia.iniciarDesenvolvimento(nacaoAtual,LinhasDeProducaoController.instancia.linhaSelecionada);
        GameController.instancia.Combustivel -= (int)custoCombustivel;
        GameController.instancia.Metal -= (int)custoMetal;
        GameController.instancia.Municao -= (int)custoMunicao;
        GameController.instancia.MaoDeObra -= (int)custoMaoDeObra;
        GameController.instancia.Inspiracao -= (int)custoInspiracao;
        if (EventosDoHQ.instancia != null) { EventosDoHQ.instancia.atualizarInterface(); }
    }

    public void mostrarCustos(){
        custoCombustivel = 2500 * nacaoAtual.modfCombustivel;
        custoMetal = 2500 * nacaoAtual.modfMetal;
        custoMunicao = 5000 * nacaoAtual.modfMunicao;
        custoMaoDeObra = 500 * nacaoAtual.modfMaoDeObra;
        custoInspiracao = 50 * nacaoAtual.modfInspiracao;

        combustivel.text = custoCombustivel.ToString();
        metal.text = custoMetal.ToString();
        municao.text = custoMunicao.ToString();
        maoDeObra.text = custoMaoDeObra.ToString();
        inspiracacao.text = custoInspiracao.ToString();
    }
}
