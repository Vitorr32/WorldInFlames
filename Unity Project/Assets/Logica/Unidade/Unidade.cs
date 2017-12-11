using UnityEngine;
using Assets.Logica.Classes;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class Unidade{
    public int idNoBancoDeDados;//Acessar facilmente a unidade no banco de dados
    public int idDaUnidade;//Id da unidade para facil identificação
    public int lvl;//Nivel atual da unidade
    public int upgradeLvl;//Nivel que permite essa unidade ser atualizada
    public int experienciaAtual;//Experiencia atual da unidade, para o proximo nivel
	public string nome;//Nome da unidade
	public int raridade;//Raridade relativa da unidade
    public int desenvolvimentoRequerido;//O quanto o desenvolvimento do pais dessa unidade e nescessario para ela ser construida
    public Classe classe;//Classe da unidade, restringe os tipos de equipamento que ela pode usar
    public string nomeDaClasse;//Nome da classe para recuperação rapida
    public Pais nacionalidade;//Nacionalidade dessa unidade
    public List<EquipamentoTipo> equipamentosNaoPermitidos;//Algumas unidades não podem usar certos equipamentos, por exemplo infantaria não pode usar uma torre de tanque
    public int maximoDeEquipamentos;//algumas unidades podem carregar mais equipamentos que outras
    public Alvos tipoDeAlvo;//Qual tipo de dano essa unidade leva, unidades armaduradas levam dano Pesado, enquanto normais levam dano macio
    public SonsDeAtaque tipoDeSom;//Cada tipo de unidade faz um tipo de som diferente
    public int idDaMordenizacao;//O id no Banco de Dados da unidade que a unidade atual atualiza
    public List<Equipamento> equipamento = new List<Equipamento>();//Os Equipamentos que essa unidade tem equipada
    /*Status da Unidades, na seguinte ordem :     
    0-HPMAX(Pontos de vida maximo do personagem, não muda)
    1-HP(Pontos de vida, se chegar a zero a unidade e dezimada)
    2-Reconhecimento(O Reconhecimento Acumulado entre os 3 Batalhoes serve pro cheque no começo da batalha)
    3-Velocidade(Em Km/h, vai ser usada na batalha pra determinar o avanço das tropas no mapa)
    4-Alcance(em metros, qual o alcance efetivo da unidade para atirar, atirar alem do alcance da um debonus de dano e precisao)
    5-Ataque Macio(Contra unidades macias: Infantaria,Cavalaria e etc)
    6-Ataque Pesado(Contra alvos com armadura, como tanques)
    7-Ataque Heroico(Contra alvos miticos)    
    8-Presicao(chance base de acerta o ataque na range ideal)
    9-Esquiva(fator que influencia na chance de acertar do inimigo)
    Calculo da esquiva > Precisão+Esquiva > Random entre 0 e Precisao+Esquiva > Se cair na precisão, acertou, caso esquiva errou
    10-Rigidez(Quanto a armadua/colete absorbe do dano, porcentagem)
    11-Perfuracao(O quanto o ataque ignora a Rigidez, porcentagem),
    Calculo da perfuração > Dano(Caso acerto) é > DefesaBruta =(Ataque*Rigidez)/100>
    >BonusPerfuracao=(AtaqueBruto*Perfuracao)/100 > AtaqueFinal = Ataque - (DefesaBruta - BonusPerfuracao)
    12-CustoCombustivel(Quanto essa unidade precisa de combustivel para chegar nos 100% de combustivel)
    13-PorcentagemDeCombustivel(Quão bem suprido a unidade está no questio combustivel)
    14-CustoMunicao(Quanto essa unidade precisa de muniçao para está 100% carregada)
    15-PorcentagemDeMunicao(Quanto porcento de munição essa unidade tem atualmente)
    16-CustoMaoDeObra(Quanto essa precisa de homens habeis a servir)
    
    Extra: Iniciativa é o qual alto no rank a unidade vai estar durante os turnos, unidades com
    alta iniciativa atacam antes das com pouca iniciativa, iniciativa e feito ao juntar o reconhecimento
    com a velocidade, para representar a detecção do inimigo e movimentação, so ataca primeiro
    quem ver e chegar primeiro    
    */
    public int[] status;
    public int[] statusBase;//Status dessa unidade sem nenhum equipamento nela
    
    public Unidade(int id, int idDaMordenizacao, int lvl, int upgradeLvl, int exp, string nome, int raridade, int desenvolvimentoRequerido, Classe classe, Pais nacionalidade, Alvos tipoDeAlvo,SonsDeAtaque tipoDeSom, int[] status, int maximoEquipamento, List<EquipamentoTipo> proibicoes){
        this.idNoBancoDeDados = id;
        this.idDaMordenizacao = idDaMordenizacao;
        this.lvl = lvl;
        this.upgradeLvl = upgradeLvl;
        this.experienciaAtual = exp;
        this.nome = nome;
        this.raridade = raridade;
        this.desenvolvimentoRequerido = desenvolvimentoRequerido;
        this.classe = classe;
        this.nacionalidade = nacionalidade;
        this.tipoDeAlvo = tipoDeAlvo;
        this.tipoDeSom = tipoDeSom;
        this.status = status;
        this.maximoDeEquipamentos = maximoEquipamento;
        this.equipamentosNaoPermitidos = proibicoes;
        statusBase = new int[status.Length];
        this.status.CopyTo(statusBase, 0);
        calcularStatusComEquipamentoELevel();
        associarIconeDaClasse();
        //associarImagensDaNacao();
    }

    public Unidade() {}

    public void expUp(int experienciaGanha){
        while(experienciaGanha > 0) {
            int expNescessaria = lvl*300;
            //Se a experiencia não é o suficiente para subir de nivel...
            if(experienciaAtual + experienciaGanha < expNescessaria){
                experienciaAtual += experienciaGanha;
                experienciaGanha=0;
            }
            //Ao contrario, se ela for o suficiente para subir de nivel
            else if(experienciaAtual+experienciaGanha >= expNescessaria){
                experienciaGanha -= expNescessaria-experienciaAtual;
                //O que restou vai ser o que sobrar da junção da experiencia atual e a conseguida agora menos o que era nescessario
                experienciaAtual=0;
                lvl++;
            }
        }
        calcularStatusComEquipamentoELevel();
    }

    public int[] calcularStatusComEquipamentoELevel(){
        int hp = status[1], pCombustivel = status[13], pMunicao = status[15];
        statusBase.CopyTo(status,0);//Resetar o status para a base da unidade

        status[0] = statusBase[0] + (statusBase[0] * (lvl - 1)) / 100;
        status[2] = statusBase[2] + (statusBase[2] * (lvl - 1)) / 100;
        status[8] = statusBase[8] + (statusBase[8] * (lvl - 1)) / 100;
        status[9] = statusBase[9] + (statusBase[9] * (lvl - 1)) / 100;

        calcularVelocidadeFinalDaDivisao(true);
        calcularAlcanceFinalDaDivisao(true);

        foreach (Equipamento equip in equipamento){
            for (int i = 0; i < equip.statusDoEquipamento.Length; i++){
                //Velocidade e alcance depennded de outros calculos não muda
                if (i == 1 || i == 3 || i == 4 || i == 13 || i == 15) { continue; } 
                status[i] += equip.statusDoEquipamento[i];
            }
        }
        status[1] = hp; status[13] = pCombustivel; status[15] = pMunicao;

        return status;
    }

    public int calcularVelocidadeFinalDaDivisao(bool summit){
        List<int> velocidades = new List<int>();

        foreach (Equipamento equip in equipamento){
            if (equip.statusDoEquipamento[3] != 0)
                velocidades.Add(equip.statusDoEquipamento[3]);
        }
        velocidades.Add(statusBase[3]);

        if(summit) status[3] = velocidades.Min();

        return velocidades.Min();
    }

    public int calcularAlcanceFinalDaDivisao(bool summit){
        List<int> alcances = new List<int>();

        foreach (Equipamento equip in equipamento){
            if (equip.statusDoEquipamento[4] != 0)
                alcances.Add(equip.statusDoEquipamento[4]);
        }
        alcances.Add(statusBase[4]);
        

        if (summit) status[4] = (int)alcances.Average();

        return (int)alcances.Average();
    }

    public int[] calcularStatusComEquipamentoTrocado(Equipamento novo, int qualSubstituir){
        
        int[] statusTemporario = new int[status.Length];
        statusBase.CopyTo(statusTemporario,0);

        statusTemporario[0] = statusBase[0] + (statusBase[0] * (lvl - 1)) / 100;
        statusTemporario[2] = statusBase[2] + (statusBase[2] * (lvl - 1)) / 100;
        statusTemporario[8] = statusBase[8] + (statusBase[8] * (lvl - 1)) / 100;
        statusTemporario[9] = statusBase[9] + (statusBase[9] * (lvl - 1)) / 100;



        for(int i=0;i<4;i++){
            for(int j=0;j<novo.statusDoEquipamento.Length;j++) {
                if(i==qualSubstituir){
                    statusTemporario[j] += novo.statusDoEquipamento[j];
                }
                else if(equipamento.Count> i){
                    statusTemporario[j] += equipamento[i].statusDoEquipamento[j];
                }
                else{
                    continue;
                }
            }
        }
        statusTemporario[3] = calcularVelocidadeFinalDaDivisao(false);
        statusTemporario[4] = calcularAlcanceFinalDaDivisao(false);

        return statusTemporario;
    }

    public void associarIconeDaClasse(){
        string caminhoBase = "Graficos/Texturas/Imagens/Classes/";
        switch (classe) {
            case Classe.Infantaria: nomeDaClasse = "Infantaria Classica"; break;
            case Classe.InfantariaNaval: nomeDaClasse = "Fuzileiro Naval"; break;
            case Classe.InfantariaAlpina: nomeDaClasse = "Infantaria Alpina"; break;
            case Classe.Artilharia: nomeDaClasse = "Artilharia"; break;
            case Classe.AntiTank: nomeDaClasse = "Antitanque"; break;
            case Classe.RocketArtilharia:  nomeDaClasse = "Artilharia de Foguetes"; break;
            case Classe.Cavalaria: nomeDaClasse = "Infantaria Montada"; break;
            case Classe.FullMecanizada:  nomeDaClasse = "Infantaria Mecanizada"; break;
            case Classe.FullMotorizada: nomeDaClasse = "Infantaria Motorizada"; break;
            case Classe.SemiMecanizada:  nomeDaClasse = "Infantaria Semi-Mecanizada"; break;
            case Classe.SemiMotorizada:  nomeDaClasse = "Infantaria Semi-Motorizada"; break;
            case Classe.TankDestroyer:  nomeDaClasse = "Antitanque automotora"; break;
            case Classe.SPArtilharia:  nomeDaClasse = "Artilharia automotora"; break;
            case Classe.SPRocketArtilharia: nomeDaClasse = "Artilharia de Foguetes automotora"; break;
            case Classe.TanqueLeve: nomeDaClasse = "Tanque Leve"; break;
            case Classe.TanqueMedio:  nomeDaClasse = "Tanque Medio"; break;
            case Classe.TanquePesado:  nomeDaClasse = "Tanque Pesado"; break;
            case Classe.TanqueUltraPesado:  nomeDaClasse = "Tanque Ultra Pesado"; break;
        }
    }


    /*
    public void associarIconeDaClasse(){
        string caminhoBase = "Graficos/Texturas/Imagens/Classes/";
        switch(classe){
            case Classe.Infantaria: classeIconePath =  caminhoBase + "Infantary" ; nomeDaClasse = "Infantaria Classica"; break;
            case Classe.InfantariaNaval: classeIconePath =  caminhoBase + "Marine" ; nomeDaClasse = "Fuzileiro Naval"; break;
            case Classe.InfantariaAlpina: classeIconePath =  caminhoBase + "Mountainner"; nomeDaClasse = "Infantaria Alpina"; break;
            case Classe.Artilharia: classeIconePath =  caminhoBase + "Artillery" ; nomeDaClasse = "Artilharia"; break;
            case Classe.AntiTank: classeIconePath =  caminhoBase + "Anti-TankGun" ; nomeDaClasse = "Antitanque"; break;
            case Classe.RocketArtilharia:classeIconePath = caminhoBase + "RocketArtillery"; nomeDaClasse = "Artilharia de Foguetes" ; break;
            case Classe.Cavalaria: classeIconePath =  caminhoBase + "Cavalry" ; nomeDaClasse = "Infantaria Montada"; break;
            case Classe.FullMecanizada: classeIconePath =  caminhoBase + "Mechanized" ; nomeDaClasse = "Infantaria Mecanizada"; break;
            case Classe.FullMotorizada:classeIconePath =  caminhoBase + "Motorized" ; nomeDaClasse = "Infantaria Motorizada"; break;
            case Classe.SemiMecanizada: classeIconePath =  caminhoBase + "SemiMechanized" ; nomeDaClasse = "Infantaria Semi-Mecanizada"; break;
            case Classe.SemiMotorizada:classeIconePath =  caminhoBase + "SemiMotorized" ; nomeDaClasse = "Infantaria Semi-Motorizada"; break;
            case Classe.TankDestroyer:classeIconePath =  caminhoBase + "TankDestroyer" ; nomeDaClasse = "Antitanque automotora"; break;
            case Classe.SPArtilharia:classeIconePath =  caminhoBase + "SelfPropelledArtillery" ; nomeDaClasse = "Artilharia automotora"; break;
            case Classe.SPRocketArtilharia: classeIconePath = caminhoBase + "SelfPropelledRocketArtillery" ; nomeDaClasse = "Artilharia de Foguetes automotora"; break;
            case Classe.TanqueLeve:classeIconePath =  caminhoBase + "LightTank" ; nomeDaClasse = "Tanque Leve"; break;
            case Classe.TanqueMedio:classeIconePath =  caminhoBase + "MediumTank" ; nomeDaClasse = "Tanque Medio"; break;
            case Classe.TanquePesado:classeIconePath =  caminhoBase + "HeavyTank" ; nomeDaClasse = "Tanque Pesado"; break;
            case Classe.TanqueUltraPesado:classeIconePath =  caminhoBase + "UltraHeavyTank" ; nomeDaClasse = "Tanque Ultra Pesado"; break;
        }
    }
    
    public void associarImagensDaNacao(){
        string caminhoBase = "Graficos/Texturas/Imagens/Paises";
        switch (nacionalidade){
            case Pais.Alemanha: bandeiraNacionalPath = caminhoBase + "ThirdReich"; nomeDaNacaoCustomizado = caminhoBase + "ThirdReichNome";break;
            case Pais.Polonia: bandeiraNacionalPath = caminhoBase + "Polonia"; nomeDaNacaoCustomizado = caminhoBase + "PoloniaNome"; break;
            case Pais.Finlandia: bandeiraNacionalPath = caminhoBase + "Finlandia"; nomeDaNacaoCustomizado = caminhoBase + "FinlandiaNome"; break;
            case Pais.EUA: bandeiraNacionalPath = caminhoBase + "EUA"; nomeDaNacaoCustomizado = caminhoBase + "EUANome"; break;
            case Pais.Franca: bandeiraNacionalPath = caminhoBase + "Franca"; nomeDaNacaoCustomizado = caminhoBase + "FrancaNome"; break;
            case Pais.Italia: bandeiraNacionalPath = caminhoBase + "Italia"; nomeDaNacaoCustomizado = caminhoBase + "ItaliaNome"; break;
            case Pais.Japao: bandeiraNacionalPath = caminhoBase + "Japao"; nomeDaNacaoCustomizado = caminhoBase + "JapaoNome"; break;
            case Pais.ReinoUnido: bandeiraNacionalPath = caminhoBase + "UK"; nomeDaNacaoCustomizado = caminhoBase + "UKNome"; break;
            case Pais.URSS: bandeiraNacionalPath = caminhoBase + "URSS"; nomeDaNacaoCustomizado = caminhoBase + "URSSNome"; break;
        }
    }
    */
}
