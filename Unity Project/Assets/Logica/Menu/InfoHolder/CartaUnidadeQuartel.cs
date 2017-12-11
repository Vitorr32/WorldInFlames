using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CartaUnidadeQuartel : MonoBehaviour {
    [HideInInspector]
    public Unidade unidadeAssociada;

    public Text level;
    public Text pontosDeVida;
    public Text nomeDaClasse;
    public Image iconeUnidadeQuartel;
    public Image iconeClasseDaUnidade;
    public Image bandeiraDaNacao;
    public Image nomeDaNacao;

    public Button botaoDeTroca;
    public Button botaoDeMandarPraReserva;

    public Slider healthPoints;
    public Slider municao;
    public Slider combustivel;
    public Slider proximoNivel;

    public Image healthPointsBar;
    public Image municaoPointsBar;
    public Image combustivelPointsBar;


    public void PopularCartaUnidadeQuartel(Unidade unidadeAssociada){
        this.unidadeAssociada = unidadeAssociada;
        level.text = unidadeAssociada.lvl.ToString();
        pontosDeVida.text = unidadeAssociada.status[1] + "/" + unidadeAssociada.status[0]; 
        iconeUnidadeQuartel.sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[unidadeAssociada.idNoBancoDeDados];
        iconeClasseDaUnidade.sprite = BancoDeDados.instancia.iconesDasClasses[(int)unidadeAssociada.classe];

        bandeiraDaNacao.sprite = BancoDeDados.instancia.iconesDeNacoes[(int)unidadeAssociada.nacionalidade];
        nomeDaNacao.sprite = BancoDeDados.instancia.nomesDeNacoes[(int)unidadeAssociada.nacionalidade];

        if (unidadeAssociada.nomeDaClasse.Length <= 19) { nomeDaClasse.fontSize = 16; }
        else { nomeDaClasse.fontSize = 11; }
        nomeDaClasse.text = unidadeAssociada.nomeDaClasse;

        healthPoints.maxValue = unidadeAssociada.status[0];
        healthPoints.value = unidadeAssociada.status[1];
        municao.maxValue = 100;
        municao.value = unidadeAssociada.status[15];

        if (municao.value <= 33){ municaoPointsBar.color = Color.red; }
        else if(municao.value < 66) { municaoPointsBar.color = new Color(255, 165, 0); }
        else { municaoPointsBar.color = Color.green; }

        combustivel.maxValue = 100;
        combustivel.value = unidadeAssociada.status[13];

        if (combustivel.value <= 33) { combustivelPointsBar.color = Color.red; }
        else if (combustivel.value < 66) { combustivelPointsBar.color = new Color(255, 165, 0); }
        else { combustivelPointsBar.color = Color.green; }

        proximoNivel.maxValue = unidadeAssociada.lvl*300;
        proximoNivel.value = unidadeAssociada.experienciaAtual;
    }
}
