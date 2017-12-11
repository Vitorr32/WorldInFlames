using UnityEngine;
using Assets.Logica.Classes;
using System.Collections.Generic;
using System;

[Serializable]
public class Construcao{
    public int id;//Acessar facilmente a unidade no banco de dados
	public string nome;//Nome da unidade
	public string iconePath;//Icone da unidade
    public bool agressiva;//Se a construção ataca(Como um bunker) ou simplesmente fica lá, como uma ponte
    /*Status da construção, são parecidos com a de Unidade, mas tem algumas alterações, na seguinte ordem : 
    HP(Pontos de vida, se chegar a zero a construção e retirada do mapa)
    Alcance(em metros, qual o alcance efetivo da unidade para atirar, atirar alem do alcance da um debonus de dano e precisao)
    Ataque Macio(Contra unidades macias: Infantaria,Cavalaria e etc)
    Ataque Pesado(Contra alvos com armadura, como tanques)
    Presicao(chance base de acerta o ataque na range ideal)
    Rigidez(Quanto a armadua/colete absorbe do dano, porcentagem)
    Perfuracao(O quanto o ataque ignora a Rigidez, porcentagem),
    Calculo da perfuração > Dano(Caso acerto) é > DefesaBruta =(Ataque*Rigidez)/100>
    >BonusPerfuracao=(AtaqueBruto*Perfuracao)/100 > AtaqueFinal = Ataque - (DefesaBruta - BonusPerfuracao)
    */
    public int[] status;

    public Construcao(int id,string nome,string iconePath,bool agressiva,int[] status){
        this.id = id;
        this.nome = nome;
        this.iconePath = iconePath;
        this.agressiva = agressiva;
        this.status = status;
    }

}
