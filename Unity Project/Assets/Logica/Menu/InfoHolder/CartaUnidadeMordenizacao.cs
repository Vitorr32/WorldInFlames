using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CartaUnidadeMordenizacao : MonoBehaviour {
    public Image iconeDaUnidade;
    public Image iconeDeClasse;
    public Image nivelDeDesenvolvimentoNescessario;
    public Sprite[] niveisTecnologicos;    

    public Text nomeDaClasse;
    public Text levelDaUnidade;
    public Text levelParaModernizacao;
    public Text[] status;

    public void popularCartaDeModernizacao(Unidade unidade, bool previa){
        iconeDaUnidade.sprite = BancoDeDados.instancia.visualSecundarioUnidadesDoJogo[unidade.idNoBancoDeDados];
        iconeDeClasse.sprite = BancoDeDados.instancia.iconesDasClasses[(int)unidade.classe];
        nomeDaClasse.text = unidade.nomeDaClasse;
        if (!previa){
            levelDaUnidade.text = unidade.lvl.ToString();
            levelParaModernizacao.text = unidade.upgradeLvl.ToString();
        }
        else{
            nivelDeDesenvolvimentoNescessario.sprite = niveisTecnologicos[unidade.desenvolvimentoRequerido];
        }

        for (int i = 0; i < 11; i++) {
            if (i == 0) { status[i].text = unidade.statusBase[i].ToString(); }
            else { status[i].text = unidade.statusBase[i + 1].ToString();}
        }
    }
    
}
