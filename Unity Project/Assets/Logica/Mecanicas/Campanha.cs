using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Logica.Classes;

public class Campanha{
    /*Uma campanha é um mapa com varios nodes em que as unidades do player podem se dirigir
    A direção que as unidades do player andam pelos nodes vai depender de varios fatores
    Como por exemplo a velocidade media das unidades, limite de classe(Um tanque pesado
    não pode ir pelo node de pantâno), poderio do inimigo e outros*/
    public int id;
    public string nome;
    public string descricao;
    public string cenaDaCampanha;
    public bool ativo = false;
    public Pais paisDaCampanha;
    public Estacao estacaoDaOfensiva;
    public int diasMaximo;
    public List<int[]> victoryConditions;
    public Limitacoes limitacoesDaCampanha;

    public Campanha(int id, string nome, Pais paisDaCampanha, Estacao estacaoDaOfensiva, string descricao, string campanha, int diasMaximo, List<int[]> victoryConditions, Limitacoes limitacao){
        this.id = id;
        this.nome = nome;
        this.paisDaCampanha = paisDaCampanha;
        this.estacaoDaOfensiva = estacaoDaOfensiva;
        this.descricao = descricao;
        this.cenaDaCampanha = campanha;
        this.diasMaximo = diasMaximo;
        this.victoryConditions = victoryConditions;
        this.limitacoesDaCampanha = limitacao;
    }
    /*
    public Campanha(int id, string nome,Pais paisDaCampanha,Estacao estacaoDaOfensiva, string descricao, string icone1,string icone2,string icone3,string campanha,int diasMaximo, List<int[]> victoryConditions, Limitacoes limitacao){
        this.id = id;
        this.nome = nome;
        this.paisDaCampanha = paisDaCampanha;
        this.estacaoDaOfensiva = estacaoDaOfensiva;
        this.descricao = descricao;
        this.demonstracaoDoMapa = icone1;
        //this.demosntracaoDoMapaDesabilidata = icone3;
        //this.iconeParaBotaoDeCampanha = icone2;
        this.cenaDaCampanha = campanha;
        this.diasMaximo = diasMaximo;
        this.victoryConditions = victoryConditions;
        this.limitacoesDaCampanha = limitacao;
    }
    */
}
