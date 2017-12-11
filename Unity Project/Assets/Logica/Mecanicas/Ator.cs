using UnityEngine;
using System.Collections;

public class Ator{
    public int id;
    public string nome;
    public Sprite[] reacoes;

    public Ator(string nome, Sprite[] reacoes){
        this.nome = nome;
        this.reacoes = reacoes;
    }
}
