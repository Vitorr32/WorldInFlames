using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CarregadorDePrefabs : MonoBehaviour{
    public static CarregadorDePrefabs instance;

	public GameObject DESERTO;
    public GameObject RUA;
    public GameObject PLANICE;
    public GameObject TERRA;
    public GameObject NEVELEVE;
    public GameObject NEVEFUNDA;
    public GameObject COLINA;
    public GameObject MONTANHA;
    public GameObject RIO;
    public GameObject GRANDERIO;
    public GameObject PONTE;
    public GameObject FLORESTA;
    public GameObject PANTANO;
    public GameObject URBANO;
    public GameObject NULA;

    public Material NO_MECHANIC;
    public Material PLAYER_ATTACK_SUPPORT;
    public Material PLAYER_RETREAT_LINE;
	public Material PLAYER_SPAWN_REINFORCEMENT;
	public Material PLAYER_SPAWN_MAIN;
    public Material ENEMY_RETREAT_LINE;
    public Material ENEMY_SPAWN_REINFORCEMENT;
    public Material ENEMY_SPAWN_SUPPORT;


    void Awake() {
		instance = this;
	}
}
