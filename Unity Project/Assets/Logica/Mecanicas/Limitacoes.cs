using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Logica.Classes;

/*Certas campanhas exigem que o player use unidades de certo tipo para deixar mais realista para com o periodo
representando pela campanha*/
public class Limitacoes{
    public int quantidadeDeUnidades;//quantas unidades de forma bruta são permitidas nessa campanha
    public List<Classe> classesProibidas = new List<Classe>();//Quais são as classes proibida de entrar nessa campanha
    public List<Vector2> classesRestringidas = new List<Vector2>();//Classes que são restritas a certa quantidade e sua quantidade

    public void adicionarRestrincao(Classe qualClasse, int quantidadeMaxima){
        classesRestringidas.Add(new Vector2((int)qualClasse, quantidadeMaxima));
    }

    public void adicionarClasseProibida(Classe qualClasse){
        classesProibidas.Add(qualClasse);
    }
    
}
