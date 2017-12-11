using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Equipamento{
    public int id;
	public string nome;
    public int raridade;
	public string descricao;
    public int[] statusDoEquipamento;
    public EquipamentoTipo tipoDeEquipamento;
    public int desenvolvimentoNescessario;
    public Pais paisDeOrigem;
    //Status do Equipamento influenciam diretamente no status da unidade equipada
    //Leia o comentario sobre Status na classe Unidade para saber mais de cada um

    public Equipamento(int id, string nome,string descricao,int desenvolvimentoNescessario, Pais paisDeOrigem,int raridade, EquipamentoTipo tipoDeEquipamento,int[] status){
        this.id = id;
        this.nome = nome;        
        this.raridade = raridade;
        this.descricao = descricao;
        this.desenvolvimentoNescessario = desenvolvimentoNescessario;
        this.paisDeOrigem = paisDeOrigem;
        this.tipoDeEquipamento = tipoDeEquipamento;
        this.statusDoEquipamento = status;
    }

    /*public void pegarCaminhoDoIcone(){
        string caminhoParaPasta = "Graficos/Texturas/Imagens/Equipamentos/TiposDeEquipamentos/";
        switch(tipoDeEquipamento){
            case EquipamentoTipo.BalaDePenetracao: equipamentoTipoSpritePath = caminhoParaPasta + "ArmorPiercingBullet"; break;
            case EquipamentoTipo.AntiTanque: equipamentoTipoSpritePath = caminhoParaPasta + "AntiTanque"; break;
            case EquipamentoTipo.RifleDeAssalto: equipamentoTipoSpritePath = caminhoParaPasta + "AssaultRifle"; break;
            case EquipamentoTipo.RifleDePrecisao: equipamentoTipoSpritePath = caminhoParaPasta + "SniperRifle"; break;
            case EquipamentoTipo.Metralhadora: equipamentoTipoSpritePath = caminhoParaPasta + "MachineGun"; break;
            case EquipamentoTipo.MetralhadoraFixa: equipamentoTipoSpritePath = caminhoParaPasta + "FixedMachineGun"; break;
            case EquipamentoTipo.BalaExplosiva: equipamentoTipoSpritePath = caminhoParaPasta + "ExplosiveBullet"; break;
            case EquipamentoTipo.BalaIcendiaria: equipamentoTipoSpritePath = caminhoParaPasta + "IncendiaryBullet"; break;
            case EquipamentoTipo.Escopeta: equipamentoTipoSpritePath = caminhoParaPasta + "ShotGun"; break;
            case EquipamentoTipo.Pistola: equipamentoTipoSpritePath = caminhoParaPasta + "HandGun"; break;
            case EquipamentoTipo.LancaChamas: equipamentoTipoSpritePath = caminhoParaPasta + "FlameThrower"; break;
            case EquipamentoTipo.TanqueLeve: equipamentoTipoSpritePath = caminhoParaPasta + "MainTurret"; break;
            case EquipamentoTipo.TanqueMedio: equipamentoTipoSpritePath = caminhoParaPasta + "SecondaryTurret"; break;
            case EquipamentoTipo.Rifle: equipamentoTipoSpritePath = caminhoParaPasta + "Rifle"; break;
        }
    }
    */

}
