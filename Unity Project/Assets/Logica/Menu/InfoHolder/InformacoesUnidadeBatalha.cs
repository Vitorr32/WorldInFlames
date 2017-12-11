using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InformacoesUnidadeBatalha : MonoBehaviour {
    public Unidade unidadeSendoMostrada;
    public Image iconeDaUnidade;
    public Image tipoDeAlvoDessaUnidade;
	public Sprite[] tiposDeAlvosSprites;
   
    public GameObject[] equipamentos = new GameObject[4];
    public Sprite equipamentoNaoAtribuido;
    public Sprite equipamentoAtribuido;
    public Sprite noClass;

    public Slider hp;
    public Slider fuel;
    public Slider ammo;
	public Image colorirHP;
	public Image colorirCombustivel;
	public Image colorirMunicao;

	public Text nomeDaClasse;
    public Text hpFeedback;
    public Text tipoDeAlvo;
    public Text ataqueMacio;
    public Text ataquePesado;
    public Text ataqueHeroico;
    public Text evasao;
    public Text precisao;

    public void popularInformacoesDaUnidadeAtual(Unidade unidade){
        unidadeSendoMostrada = unidade;
        hp.maxValue =unidade.status[0];
        hp.value = unidade.status[1];

        fuel.value = unidade.status[13];
        fuel.maxValue = 100;
        ammo.value = unidade.status[15];
        ammo.maxValue = 100;
        colorirTudo (unidade);

		iconeDaUnidade.sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[unidade.idNoBancoDeDados];
        nomeDaClasse.text = unidade.nomeDaClasse;
        hpFeedback.text = unidade.status[1]+"/"+unidade.status[0];
		if(unidade.tipoDeAlvo == Alvos.Macio) {tipoDeAlvo.text = "SOFT"; tipoDeAlvoDessaUnidade.sprite = tiposDeAlvosSprites [0]; }
		else if(unidade.tipoDeAlvo == Alvos.Duro) {tipoDeAlvo.text = "HARD"; tipoDeAlvoDessaUnidade.sprite = tiposDeAlvosSprites [1];}
		else if(unidade.tipoDeAlvo == Alvos.Heroico) {tipoDeAlvo.text = "MIST"; tipoDeAlvoDessaUnidade.sprite = tiposDeAlvosSprites [2];}
        ataqueMacio.text = unidade.status[5].ToString();
        ataquePesado.text = unidade.status[6].ToString();
        ataqueHeroico.text = unidade.status[7].ToString();
        precisao.text = unidade.status[8].ToString();
        evasao.text = unidade.status[9].ToString();
        
        atualizarEquipamentos(unidade);
    }

    public void atualizarEquipamentos(Unidade ChosenOne){
        for(int i = 0; i < 4; i++){
            if (i < ChosenOne.equipamento.Count && !ChosenOne.equipamento[i].Equals(null)){
                equipamentos[i].GetComponentInChildren<Text>().text = ChosenOne.equipamento[i].nome;
                equipamentos[i].transform.GetChild(1).GetComponent<Image>().sprite = BancoDeDados.instancia.iconesDeTiposDeEquipamentos[(int)ChosenOne.equipamento[i].tipoDeEquipamento];
                equipamentos[i].GetComponent<Image>().sprite = equipamentoAtribuido;
            }
            else{
                equipamentos[i].GetComponentInChildren<Text>().text = "";
                equipamentos[i].transform.GetChild(1).GetComponent<Image>().sprite = noClass;
                equipamentos[i].GetComponent<Image>().sprite = equipamentoNaoAtribuido;
            }
        }
    }

    public void atualizarSliders(Unidade unidade){
        hp.value = unidade.status[1];
        hpFeedback.text = unidade.status[1]+"/"+unidade.status[0];
        fuel.value = unidade.status[13];
        ammo.value = unidade.status[15];
		colorirTudo (unidade);
    }

	public void colorirTudo(Unidade unidade){
		colorirSliderDeHp (unidade);
		colorirSlider(unidade.status[13], colorirCombustivel);
		colorirSlider(unidade.status[15], colorirMunicao);
	}

	public void colorirSlider(int valor, Image aColorir){
		if(valor<=25){
			aColorir.color = Color.red;
		}else if (valor<=50){
			aColorir.color = new Color(255,165,0);
		}else if (valor<=75){
			aColorir.color = Color.yellow;
		}else aColorir.color = Color.green;	
	}
    
    public void colorirSliderDeHp(Unidade unidade){
        if(unidade.status[1] <= (unidade.status[0] * 25) / 100){
			colorirHP.color = Color.red;
        }else if (unidade.status[1] <= (unidade.status[0] * 50) / 100){
			colorirHP.color = new Color(255,165,0);
		}else if (unidade.status[1] <= (unidade.status[0] * 75) / 100){
			colorirHP.color = Color.yellow;
		}else colorirHP.color = Color.green;
    }
    
}
