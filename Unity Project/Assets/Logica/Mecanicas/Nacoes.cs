using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Nacoes{
    public Pais pais;//Qual pais esta sendo representado por esse desenvolvimento
    public string nome;
    public int nivel = 0;//Nivel tecnologico atual da nação
    public int nivelLimite;//Nivel tecnologico limite da nação
    //public bool ativa;//Se essa nação está bloqueada ao acesso do player
    public bool emDesenvolvimento;//Se está atualmente aumentando seu nivel de desenvolvimento    
    public string descricao;//Descricao da nação e seus pontos fortes e fracos
    public string[] descricoesTecnologicas;//Descrição de cada nivel tecnologico e seus efeitos
    
    //Modificadores de recursos que essa nação oferece, algumas nescessitam mais recursos que outros para recrutar
    public float modfCombustivel;
    public float modfMetal;
    public float modfMunicao;
    public float modfMaoDeObra;
    public float modfInspiracao;
}
