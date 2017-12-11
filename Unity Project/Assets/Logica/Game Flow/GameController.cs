using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Logica.Classes;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Classe responsavel por guardar informações estaticas do jogo
//Incluindo = IDs das unidades, levels, id dos equipamentos, alocação e formação
public class GameController : MonoBehaviour{
    public static GameController instancia;
    
    public int campanhaAtiva = 0; 
    public List<Unidade> reservas = new List<Unidade>(); //Aqui se armazena as unidades do player, também se encontra os equipamentos atualmente equipados neles
    public List<Equipamento> equipamentos = new List<Equipamento>(); //Aqui ser armazena os equipamentos não atualmente equipados
    public List<LinhaDeProducao> linhasDeProducao = new List<LinhaDeProducao>();//Quantas e quais linhas de produção o player pode usar
    public List<Nacoes> nacoesDoJogador = new List<Nacoes>();//Nações e seus niveis de desenvolvimento
    //Separando as unidades do player em três "linhas" de batalha
    public List<Unidade> primeiroBatalhao = new List<Unidade>();
    public List<Unidade> segundoBatalhao = new List<Unidade>();
    public List<Unidade> terceiroBatalhao = new List<Unidade>();

    public Node nodeAtual;
    private int idCont = 0;

    public int campanhaSelecionada;
    public bool rushFront = false;
    public bool onBattle = false;
    public bool victory = false;
    public int diasEmBatalha= 0;
    
    public int Metal;
    public int Municao;
    public int MaoDeObra;
    public int Combustivel;
    public int Inspiracao;
    
    void Awake(){
        if(instancia==null)instancia = this;
    }

	void Start () {
        DontDestroyOnLoad(transform.gameObject);
    }

    //------------------------UTILITIES--------------------------------
    public void organizarEquipamentoPorTipo(){
        equipamentos = equipamentos.OrderBy(m => m.tipoDeEquipamento).ThenBy(m => m.nome).ToList();
    }

    public void organizarUnidadesPorTipo(){
        reservas = reservas.OrderBy(r => r.classe).ThenBy(r => r.nome).ToList();
    }

    /* IMPORTANT*/
    public int AdicionarUnidadeAoJogador(Unidade UnidadeParaAdicionar){
        if (reservas.Count >= 30 || UnidadeParaAdicionar == null) return -1;

        UnidadeParaAdicionar.idDaUnidade = idCont;
        reservas.Add(UnidadeParaAdicionar);

        idCont++;
        return UnidadeParaAdicionar.idDaUnidade;
    }

    //--------------------------------------ADMINISTRAR SAVE/LOAD-------------------------------
    [Serializable]
    public class UnidadeData{
        public int idBD;
        public int id;
        public int lvl;
        public int exp;
        public int batalhao;
        public int[] equipamentosID;

        public UnidadeData(int idBD,int id, int batalhao,int lvl, int exp, int[] equipamentos){
            this.idBD = idBD;
            this.id = id;
            this.batalhao = batalhao;
            this.lvl = lvl;
            this.exp = exp;
            this.equipamentosID = equipamentos;
        }
    }
    [Serializable]
    public class EquipamentoData{
        public int id;
        public EquipamentoData(int id){
            this.id = id;
        }
    }
    [Serializable]
    public class GameData{
        public int idCont;
        public int campanhaAtiva;
        public int metal;
        public int combustivel;
        public int maoDeObra;
        public int inspiracao;
        public int municao;
    }
    [Serializable]
    public class NationsData{
        public int Pais;
        public int nivelAtual;
    }
    [Serializable]
    public class LinhaDeProducaoData{
        public bool ativa;
        public int tipoDeProducao;
        public float ppm;
        public float proguessoAtual;
        public int idDaUnidadeOuDesenvolvimentoOuEquipamento;
    }


    [Serializable()]
    public class SaveData : ISerializable{

        //VALORES QUE SERÃO SALVOS, APENAS COSIAS IMPROTANTES VIU
        List<UnidadeData> unidadesDoJogador;
        List<EquipamentoData> equipamentosDoJogador;
        GameData informacoesDoJogo;
        List<NationsData> informacoesDasNacoes;
        List<LinhaDeProducaoData> linhasDeProducao;

        public SaveData() { }

        public void recuperarInformacoes() {
            //Começando pelas unidades do jogador...
            foreach(UnidadeData uniData in unidadesDoJogador){
                Unidade novaUnidade = BancoDeDados.instancia.retornarUnidade(uniData.idBD);
                //Reestabelecendo dados básicos
                novaUnidade.lvl = uniData.lvl;
                novaUnidade.idDaUnidade = uniData.id;
                novaUnidade.experienciaAtual = uniData.exp;
                //Recuperando equipamentos equipados na unidades
                foreach(int idDoEquipamento in uniData.equipamentosID){
                    if (idDoEquipamento != -1){
                        Equipamento equipamentoEquipado = BancoDeDados.instancia.retornarEquipamento(idDoEquipamento);
                        novaUnidade.equipamento.Add(equipamentoEquipado);
                    }
                }
                //Deployando unidade no batalhão que estava anteriormente
                switch (uniData.batalhao){
                    case 1: GameController.instancia.primeiroBatalhao.Add(novaUnidade); break;
                    case 2: GameController.instancia.segundoBatalhao.Add(novaUnidade); break;
                    case 3: GameController.instancia.terceiroBatalhao.Add(novaUnidade); break;
                    case 4: GameController.instancia.reservas.Add(novaUnidade); break;
                }
            }
            //Agora os equipamentos...
            //Bem simples, recupear o equipamento pelo ID então coloca-lo no pool de equipamentos do jogador
            foreach(EquipamentoData equipData in equipamentosDoJogador){
                Equipamento equipamentoIdle = BancoDeDados.instancia.retornarEquipamento(equipData.id);
                GameController.instancia.equipamentos.Add(equipamentoIdle);
            }
            //Agora as informações gerais do jogo, como recursos, campanha ativa e etc.
            GameController.instancia.Metal = informacoesDoJogo.metal;
            GameController.instancia.Combustivel = informacoesDoJogo.combustivel;
            GameController.instancia.MaoDeObra = informacoesDoJogo.maoDeObra;
            GameController.instancia.Municao = informacoesDoJogo.municao;
            GameController.instancia.Inspiracao = informacoesDoJogo.inspiracao;
            GameController.instancia.idCont = informacoesDoJogo.idCont;
            GameController.instancia.setarCampanhaAtiva(informacoesDoJogo.campanhaAtiva);
            GameController.instancia.ativarCampanhas();            

            //Agora as informações dos desenvolvimentos das nações
            foreach(NationsData natData in informacoesDasNacoes){
                Nacoes novaNacao = BancoDeDados.instancia.retornarNacao((Pais)natData.Pais);
                novaNacao.nivel = natData.nivelAtual;
                GameController.instancia.nacoesDoJogador.Add(novaNacao);
            }

            //Agora as informações de linhas de desenvolvimento
            //Parece complicado mas é nescessariamente : Ativa > Continuar producção, Inativa > Do nothing
            foreach(LinhaDeProducaoData lData in linhasDeProducao){
                LinhaDeProducao linhaNova = new LinhaDeProducao();
                linhaNova.ppm = lData.ppm;
                linhaNova.proguesso = lData.proguessoAtual;
                linhaNova.tipoDeProducao = (ProducaoTipo)lData.tipoDeProducao;
                linhaNova.ativa = lData.ativa;
                switch (linhaNova.tipoDeProducao){
                    case ProducaoTipo.Inativa:
                        break;
                    case ProducaoTipo.Desenvolvimento:
                        linhaNova.nacaoSendoDesenvolvida = instancia.nacoesDoJogador.First(nation => nation.pais == (Pais)lData.idDaUnidadeOuDesenvolvimentoOuEquipamento);
                        instancia.despausarProducao(linhaNova);
                        break;
                    case ProducaoTipo.Construcao:
                        linhaNova.equipamentoSendoConstruido = BancoDeDados.instancia.retornarEquipamento(lData.idDaUnidadeOuDesenvolvimentoOuEquipamento);
                        instancia.despausarProducao(linhaNova);
                        break;
                    case ProducaoTipo.Recrutamento:
                        linhaNova.unidadeSendoProduzida = BancoDeDados.instancia.retornarUnidade(lData.idDaUnidadeOuDesenvolvimentoOuEquipamento);
                        instancia.despausarProducao(linhaNova);
                        break;
                    case ProducaoTipo.Combustivel:
                    case ProducaoTipo.Metais:
                    case ProducaoTipo.Municao:
                    case ProducaoTipo.BensDeConsumo:
                        instancia.iniciarProducaoDeRecursos(linhaNova);
                        break;
                }
                instancia.linhasDeProducao.Add(linhaNova);
                if(linhaNova.tipoDeProducao != ProducaoTipo.Inativa) GameController.instancia.despausarProducao(linhaNova);
            }

        }

        public void arquivarInformacoes(){
            List<UnidadeData> unidadesDoPlayer = new List<UnidadeData>();
            if (GameController.instancia.primeiroBatalhao.Count != 0){
                foreach(Unidade u in GameController.instancia.primeiroBatalhao){
                    unidadesDoPlayer.Add(new UnidadeData(u.idNoBancoDeDados,u.idDaUnidade, 1, u.lvl, u.experienciaAtual, idDosEquipamentos(u)));
                }
            }
            if (GameController.instancia.segundoBatalhao.Count != 0){
                foreach (Unidade u in GameController.instancia.segundoBatalhao){
                    unidadesDoPlayer.Add(new UnidadeData(u.idNoBancoDeDados, u.idDaUnidade, 2, u.lvl, u.experienciaAtual, idDosEquipamentos(u)));
                }
            }
            if (GameController.instancia.terceiroBatalhao.Count != 0){
                foreach (Unidade u in GameController.instancia.terceiroBatalhao){
                    unidadesDoPlayer.Add(new UnidadeData(u.idNoBancoDeDados, u.idDaUnidade, 3, u.lvl, u.experienciaAtual, idDosEquipamentos(u)));
                }
            }
            if (GameController.instancia.reservas.Count != 0){
                foreach (Unidade u in GameController.instancia.reservas){
                    unidadesDoPlayer.Add(new UnidadeData(u.idNoBancoDeDados, u.idDaUnidade, 4, u.lvl, u.experienciaAtual, idDosEquipamentos(u)));
                }
            }
            unidadesDoJogador = unidadesDoPlayer;


            List<EquipamentoData> listaDeEquips = new List<EquipamentoData>();
            foreach(Equipamento equip in GameController.instancia.equipamentos){
                listaDeEquips.Add(new EquipamentoData(equip.id));
            }
            equipamentosDoJogador = listaDeEquips;


            GameData gameInfo = new GameData();

            gameInfo.campanhaAtiva = GameController.instancia.campanhaAtiva;
            gameInfo.combustivel = GameController.instancia.Combustivel;
            gameInfo.municao = GameController.instancia.Municao;
            gameInfo.metal = GameController.instancia.Metal;
            gameInfo.maoDeObra = GameController.instancia.MaoDeObra;
            gameInfo.inspiracao = GameController.instancia.Inspiracao;
            gameInfo.idCont = GameController.instancia.idCont;

            informacoesDoJogo = gameInfo;


            List<NationsData> nationsInfo = new List<NationsData>();

            foreach (Nacoes dens in GameController.instancia.nacoesDoJogador){
                NationsData newNation = new NationsData();
                newNation.Pais = (int)dens.pais;
                newNation.nivelAtual = dens.nivel;
                nationsInfo.Add(newNation);
            }

            informacoesDasNacoes = nationsInfo;


            List<LinhaDeProducaoData> prodInfo = new List<LinhaDeProducaoData>();

            foreach (LinhaDeProducao linha in GameController.instancia.linhasDeProducao){
                LinhaDeProducaoData novaLinha = new LinhaDeProducaoData();
                switch (linha.tipoDeProducao){
                    case ProducaoTipo.Desenvolvimento:
                        novaLinha.idDaUnidadeOuDesenvolvimentoOuEquipamento = (int)linha.nacaoSendoDesenvolvida.pais;
                        break;
                    case ProducaoTipo.Construcao:
                        novaLinha.idDaUnidadeOuDesenvolvimentoOuEquipamento = linha.equipamentoSendoConstruido.id;
                        break;
                    case ProducaoTipo.Recrutamento:
                        novaLinha.idDaUnidadeOuDesenvolvimentoOuEquipamento = linha.unidadeSendoProduzida.idNoBancoDeDados;
                        break;
                }
                if(linha.tipoDeProducao != ProducaoTipo.Inativa) novaLinha.ativa = true;
                novaLinha.tipoDeProducao = (int)linha.tipoDeProducao;
                novaLinha.ppm = linha.ppm;
                novaLinha.proguessoAtual = linha.proguesso;
                prodInfo.Add(novaLinha);
            }
            linhasDeProducao = prodInfo;
        }

        public int[] idDosEquipamentos(Unidade u) {
            int[] idDosEquipamentos = new int[u.maximoDeEquipamentos];
            for(int i =0; i< u.maximoDeEquipamentos;i++){
                if (i < u.equipamento.Count){
                    idDosEquipamentos[i] = u.equipamento[i].id;
                }
                else{
                    idDosEquipamentos[i] = -1;
                }
            }
            return idDosEquipamentos;
        }
 
        public SaveData(SerializationInfo info, StreamingContext ctxt){
            // Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
            // Do this for each var defined in the Values section above
            unidadesDoJogador = (List<UnidadeData>)info.GetValue("unidadesDoJogador", typeof(List<UnidadeData>));
            equipamentosDoJogador = (List<EquipamentoData>)info.GetValue("equipamentosDoJogador", typeof(List<EquipamentoData>));
            informacoesDoJogo = (GameData)info.GetValue("informacoesDoJogo", typeof(GameData));
            informacoesDasNacoes = (List<NationsData>)info.GetValue("informacoesDasNacoes", typeof(List<NationsData>));
            linhasDeProducao = (List<LinhaDeProducaoData>)info.GetValue("linhasDeProducao", typeof(List<LinhaDeProducaoData>));
            recuperarInformacoes();
        }

        // Required by the ISerializable class to be properly serialized. This is called automatically
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt){
            // Repeat this for each var defined in the Values section
            arquivarInformacoes();
            info.AddValue("unidadesDoJogador", unidadesDoJogador);
            info.AddValue("equipamentosDoJogador", equipamentosDoJogador);
            info.AddValue("informacoesDoJogo", informacoesDoJogo);
            info.AddValue("informacoesDasNacoes", informacoesDasNacoes);
            info.AddValue("linhasDeProducao", linhasDeProducao);
        }
    }

    public class SaveLoad{

        public static string currentFilePath = "SaveData.awif";    // Edit this for different save files

        // Call this to write data
        public static void Save(){ Save(currentFilePath);}
        public static void Save(string filePath){
            SaveData data = new SaveData();

            Stream stream = File.Open(filePath, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Binder = new VersionDeserializationBinder();
            bformatter.Serialize(stream, data);
            stream.Close();
        }

        // Call this to load from a file into "data"
        public static void Load() { Load(currentFilePath); }   // Overloaded
        public static void Load(string filePath) {
            SaveData data = new SaveData();
            Stream stream = File.Open(filePath, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Binder = new VersionDeserializationBinder();
            data = (SaveData)bformatter.Deserialize(stream);
            stream.Close();

            // Now use "data" to access your Values
        }

    }


    public sealed class VersionDeserializationBinder : SerializationBinder{
        public override Type BindToType(string assemblyName, string typeName){
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName)){
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type. 
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

                return typeToDeserialize;
            }

            return null;
        }
    }



        //----------------------------ATIVAR CAMPANHAS----------------------------------------

    public void setarCampanhaAtiva(int campanhaAtiva){
        this.campanhaAtiva = campanhaAtiva;
    }

    public void ativarCampanhas(){
        for(int i = 0; i < BancoDeDados.instancia.campanhasDoJogo.Count; i++){
            if (i <= campanhaAtiva){
                BancoDeDados.instancia.campanhasDoJogo[i].ativo = true;
            }
            else{
                BancoDeDados.instancia.campanhasDoJogo[i].ativo = false;
            }
        }
    }
    //-------------------------------DESENVOLVIMETNO/CONSTRUÇÃO/DROPS------------------------
    //Construção a partir de uma linha de produção, unico jeito de conseguir equipamentos 5*
    public Equipamento Construcao(int nivelDeInvestimento, Nacoes desenvolvimentoDoPais){
        int numeroAleatorio = UnityEngine.Random.Range(0, 100);
        IEnumerable<Equipamento> equipamentosPossiveis = BancoDeDados.instancia.equipamentosDoJogo.Where(x => (x.paisDeOrigem == desenvolvimentoDoPais.pais) && (x.desenvolvimentoNescessario <= desenvolvimentoDoPais.nivel));
        if (equipamentosPossiveis.Count() == 0) { return null; }
        switch (nivelDeInvestimento){
            case 4://Investimento de elite na construção de equipamento, 6% de chance de equipamento 5*
                if (numeroAleatorio > 94){
                    IEnumerable<Equipamento> equipamento5stars = equipamentosPossiveis.Where(x => x.raridade == 5);
                    int qual = UnityEngine.Random.Range(0, equipamento5stars.Count());
                    if (equipamento5stars.Count() != 0) {
                        return equipamento5stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 84){
                    IEnumerable<Equipamento> equipamento4stars = equipamentosPossiveis.Where(x => x.raridade == 4);
                    int qual = UnityEngine.Random.Range(0, equipamento4stars.Count());
                    if (equipamento4stars.Count() != 0){
                        return equipamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 64){
                    IEnumerable<Equipamento> equipamento3stars = equipamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, equipamento3stars.Count());
                    if (equipamento3stars.Count() != 0){
                        return equipamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 34){
                    IEnumerable<Equipamento> equipamento2stars = equipamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, equipamento2stars.Count());
                    if (equipamento2stars.Count() != 0){
                        return equipamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 14){
                    IEnumerable<Equipamento> equipamento1star = equipamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, equipamento1star.Count());
                    if (equipamento1star.Count() != 0){
                        return equipamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 3://Investimento avançado na construção de equipamento 2% de chance de equipamento 5*
                if (numeroAleatorio > 98){
                    IEnumerable<Equipamento> equipamento5stars = equipamentosPossiveis.Where(x => x.raridade == 5);
                    int qual =UnityEngine.Random.Range(0, equipamento5stars.Count());
                    if (equipamento5stars.Count() != 0){
                        return equipamento5stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 90){
                    IEnumerable<Equipamento> equipamento4stars = equipamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, equipamento4stars.Count());
                    if (equipamento4stars.Count() != 0){
                        return equipamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 74){
                    IEnumerable<Equipamento> equipamento3stars = equipamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, equipamento3stars.Count());
                    if (equipamento3stars.Count() != 0){
                        return equipamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 54){
                    IEnumerable<Equipamento> equipamento2stars = equipamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, equipamento2stars.Count());
                    if (equipamento2stars.Count() != 0){
                        return equipamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 29){
                    IEnumerable<Equipamento> equipamento1star = equipamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, equipamento1star.Count());
                    if (equipamento1star.Count() != 0){
                        return equipamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 2://Investimento de elite na construção de equipamento, sem chance de equipamento 5*
                if (numeroAleatorio > 89){
                    IEnumerable<Equipamento> equipamento4stars = equipamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, equipamento4stars.Count());
                    if (equipamento4stars.Count() != 0){
                        return equipamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 70){
                    IEnumerable<Equipamento> equipamento3stars = equipamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, equipamento3stars.Count());
                    if (equipamento3stars.Count() != 0){
                        return equipamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 50){
                    IEnumerable<Equipamento> equipamento2stars = equipamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, equipamento2stars.Count());
                    if (equipamento2stars.Count() != 0) {
                        return equipamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 35){
                    IEnumerable<Equipamento> equipamento1star = equipamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, equipamento1star.Count());
                    if (equipamento1star.Count() != 0){
                        return equipamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 1:
                if (numeroAleatorio > 98) {
                    IEnumerable<Equipamento> equipamento4stars = equipamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, equipamento4stars.Count());
                    if (equipamento4stars.Count() != 0){
                        return equipamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 80) {
                    IEnumerable<Equipamento> equipamento3stars = equipamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, equipamento3stars.Count());
                    if (equipamento3stars.Count() != 0){
                        return equipamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 65){
                    IEnumerable<Equipamento> equipamento2stars = equipamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, equipamento2stars.Count());
                    if (equipamento2stars.Count() != 0){
                        return equipamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 45) {
                    IEnumerable<Equipamento> equipamento1star = equipamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, equipamento1star.Count());
                    if (equipamento1star.Count() != 0){
                        return equipamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 0:
                if (numeroAleatorio > 90){
                    IEnumerable<Equipamento> equipamento3stars = equipamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, equipamento3stars.Count());
                    if (equipamento3stars.Count() != 0){
                        return equipamento3stars.ElementAt(qual);
                    }
                }
                else if (numeroAleatorio > 75){
                    IEnumerable<Equipamento> equipamento2stars = equipamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, equipamento2stars.Count());
                    if (equipamento2stars.Count() != 0){
                        return equipamento2stars.ElementAt(qual);
                    }
                }
                else if (numeroAleatorio > 60){
                    IEnumerable<Equipamento> equipamento1star = equipamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, equipamento1star.Count());
                    if (equipamento1star.Count() != 0){
                        return equipamento1star.ElementAt(qual);
                    }
                }
                return null;
        }
        return null;
    }

    //Recrutamento a partir de uma linha de produção, unico jeito de conseguir unidades 5*
    public Unidade Recrutamento(int nivelDeInvestimento, Nacoes desenvolvimentoDoPais){
        int numeroAleatorio = UnityEngine.Random.Range(0, 100);
        IEnumerable<Unidade> recrutamentosPossiveis = BancoDeDados.instancia.unidadesDoJogo.Where(x => (x.nacionalidade == desenvolvimentoDoPais.pais) && (x.desenvolvimentoRequerido <= desenvolvimentoDoPais.nivel));
        if (recrutamentosPossiveis.Count() == 0) { return null; }

        switch (nivelDeInvestimento){
            case 4://Investimento Muito Alto no recrutamento
                if (numeroAleatorio > 94){
                    IEnumerable<Unidade> recrutamento5stars = recrutamentosPossiveis.Where(x => x.raridade == 5);
                    int qual =UnityEngine.Random.Range(0, recrutamento5stars.Count());
                    if (recrutamento5stars.Count() != 0){
                        return recrutamento5stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 84){
                    IEnumerable<Unidade> recrutamento4stars = recrutamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, recrutamento4stars.Count());
                    if (recrutamento4stars.Count() != 0){
                        return recrutamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 64){
                    IEnumerable<Unidade> recrutamento3stars = recrutamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, recrutamento3stars.Count());
                    if (recrutamento3stars.Count() != 0) {
                        return recrutamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 34){
                    IEnumerable<Unidade> recrutamento2stars = recrutamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, recrutamento2stars.Count());
                    if (recrutamento2stars.Count() != 0){
                        return recrutamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 14){
                    IEnumerable<Unidade> recrutamento1star = recrutamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, recrutamento1star.Count());
                    if (recrutamento1star.Count() != 0){
                        return recrutamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 3:
                if (numeroAleatorio > 98){
                    IEnumerable<Unidade> recrutamento5stars = recrutamentosPossiveis.Where(x => x.raridade == 5);
                    int qual =UnityEngine.Random.Range(0, recrutamento5stars.Count());
                    if (recrutamento5stars.Count() != 0){
                        return recrutamento5stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 80){
                    IEnumerable<Unidade> recrutamento4stars = recrutamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, recrutamento4stars.Count());
                    if (recrutamento4stars.Count() != 0){
                        return recrutamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 50){
                    IEnumerable<Unidade> recrutamento3stars = recrutamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, recrutamento3stars.Count());
                    if (recrutamento3stars.Count() != 0){
                        return recrutamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 30){
                    IEnumerable<Unidade> recrutamento2stars = recrutamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, recrutamento2stars.Count());
                    if (recrutamento2stars.Count() != 0){
                        return recrutamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 20){
                    IEnumerable<Unidade> recrutamento1star = recrutamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, recrutamento1star.Count());
                    if (recrutamento1star.Count() != 0){
                        return recrutamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 2:
                if (numeroAleatorio > 89){
                    IEnumerable<Unidade> recrutamento4stars = recrutamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, recrutamento4stars.Count());
                    if (recrutamento4stars.Count() != 0){
                        return recrutamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 70){
                    IEnumerable<Unidade> recrutamento3stars = recrutamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, recrutamento3stars.Count());
                    if (recrutamento3stars.Count() != 0){
                        return recrutamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 50){
                    IEnumerable<Unidade> recrutamento2stars = recrutamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, recrutamento2stars.Count());
                    if (recrutamento2stars.Count() != 0){
                        return recrutamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 45){
                    IEnumerable<Unidade> recrutamento1star = recrutamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, recrutamento1star.Count());
                    if (recrutamento1star.Count() != 0){
                        return recrutamento1star.ElementAt(qual);
                    }
                }

                return null;
            case 1:
                if (numeroAleatorio > 98){
                    IEnumerable<Unidade> recrutamento4stars = recrutamentosPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, recrutamento4stars.Count());
                    if (recrutamento4stars.Count() != 0){
                        return recrutamento4stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 80){
                    IEnumerable<Unidade> recrutamento3stars = recrutamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, recrutamento3stars.Count());
                    if (recrutamento3stars.Count() != 0){
                        return recrutamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 65){
                    IEnumerable<Unidade> recrutamento2stars = recrutamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, recrutamento2stars.Count());
                    if (recrutamento2stars.Count() != 0){
                        return recrutamento2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 35){
                    IEnumerable<Unidade> recrutamento1star = recrutamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, recrutamento1star.Count());
                    if (recrutamento1star.Count() != 0){
                        return recrutamento1star.ElementAt(qual);
                    }
                }
                return null;
            case 0:
                if (numeroAleatorio > 90){
                    IEnumerable<Unidade> recrutamento3stars = recrutamentosPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0, recrutamento3stars.Count());
                    if (recrutamento3stars.Count() != 0){
                        return recrutamento3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 75){
                    IEnumerable<Unidade> recrutamento2stars = recrutamentosPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0, recrutamento2stars.Count());
                    if (recrutamento2stars.Count() != 0){
                        return recrutamento2stars.ElementAt(qual);
                    }
                }
                if(numeroAleatorio > 60){
                    IEnumerable<Unidade> recrutamento1star = recrutamentosPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0, recrutamento1star.Count());
                    if (recrutamento1star.Count() != 0){
                        return recrutamento1star.ElementAt(qual);
                    }
                }
                return null;
        }
        return null;
    }//Uniade que o jogador consegue recrutar dependendo do quanto ele investiu e o desenvolvimento do pais

    //Drop de uma batalha, a qualiade do drop dependerá do desempenho da batalha, por isso o recebe como parametro
    //Outro parametro "escondido" e a nacionalidade da unidade, que e retirada da variavel nodeAtual aqui no GameController
    public Unidade Drop(int battleRating){
        if(reservas.Count == 30) { return null; }
        int numeroAleatorio = UnityEngine.Random.Range(0,100);
        IEnumerable<Unidade> dropsPossiveis = BancoDeDados.instancia.unidadesDoJogo.Where(x => x.nacionalidade == nodeAtual.nacionalidade);
        //O switch so vai até 3, pois a partir dai são resultados de batalhas ruims (empate ou derrota)
        switch (battleRating){
            case 5://Apenas no S Rank temos drops 4 estrelas
                if(numeroAleatorio > 99){
                    IEnumerable<Unidade> drop4stars = dropsPossiveis.Where(x => x.raridade == 4);
                    int qual =UnityEngine.Random.Range(0, drop4stars.Count());
                    if (drop4stars.Count() != 0){
                        return drop4stars.ElementAt(qual);
                    }
                }
                if(numeroAleatorio > 90){
                    IEnumerable<Unidade> drop3stars = dropsPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0,drop3stars.Count());
                    if (drop3stars.Count() != 0){
                        return drop3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 85){
                    IEnumerable<Unidade> drop2stars = dropsPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0,drop2stars.Count());
                    if (drop2stars.Count() != 0){
                        return drop2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 50){
                    IEnumerable<Unidade> drop1stars = dropsPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0,drop1stars.Count());
                    if (drop1stars.Count() != 0){
                        return drop1stars.ElementAt(qual);
                    }
                }
                return null;
            case 4:
                if(numeroAleatorio > 99){
                    IEnumerable<Unidade> drop3stars = dropsPossiveis.Where(x => x.raridade == 3);
                    int qual =UnityEngine.Random.Range(0,drop3stars.Count());
                    if (drop3stars.Count() != 0){
                        return drop3stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 90){
                    IEnumerable<Unidade> drop2stars = dropsPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0,drop2stars.Count());
                    if (drop2stars.Count() != 0){
                        return drop2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 75){
                    IEnumerable<Unidade> drop1stars = dropsPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0,drop1stars.Count());
                    if (drop1stars.Count() != 0){
                        return drop1stars.ElementAt(qual);
                    }
                }
                return null;
            case 3:
                if(numeroAleatorio > 94){
                    IEnumerable<Unidade> drop2stars = dropsPossiveis.Where(x => x.raridade == 2);
                    int qual =UnityEngine.Random.Range(0,drop2stars.Count());
                    if (drop2stars.Count() != 0){
                        return drop2stars.ElementAt(qual);
                    }
                }
                if (numeroAleatorio > 75){
                    IEnumerable<Unidade> drop1stars = dropsPossiveis.Where(x => x.raridade == 1);
                    int qual =UnityEngine.Random.Range(0,drop1stars.Count());
                    if (drop1stars.Count() != 0) {
                        return drop1stars.ElementAt(qual);
                    }
                }
                return null;
        }
        return null;
    }//Calculo complexo de qual unidade o player conseguiu como drop

    public void darExperienciaUnidades(int experiencia){
        foreach(Unidade u in primeiroBatalhao){
                u.expUp(experiencia);
            }
        if(segundoBatalhao.Count!=0){
            foreach(Unidade u in segundoBatalhao){
                u.expUp(experiencia);
            }
        }
        if(terceiroBatalhao.Count!=0){
            foreach(Unidade u in terceiroBatalhao){
                u.expUp(experiencia);
            }
        }
    }//Quando uma batalha acaba, as unidades que participaram dela recebem experiencia

    public void iniciarProducaoDeRecursos(LinhaDeProducao linha){
        linha.proguesso = 100;
        linha.ppm = 0;
        linha.terminada = false;
        linha.emEspera = false;
        linha.ativa = true;
        StartCoroutine(linha.produzir());
        LinhasDeProducaoController.instancia.atualizarVisual(linha);
    }

    public void iniciarConstrucao(Equipamento equipamentoConstruido, LinhaDeProducao linha){
        linha.proguesso = 0;
        linha.ppm = 100 / calculoDeTempoDeConstrucaoDeEquipamento(equipamentoConstruido);
        linha.terminada = false;
        linha.emEspera = false;
        linha.ativa = true;
        linha.equipamentoSendoConstruido = equipamentoConstruido;
        StartCoroutine(linha.produzir());
        LinhasDeProducaoController.instancia.atualizarVisual(linha);
    }

    public void iniciarRecrutamento(Unidade unidadeRecrutada, LinhaDeProducao linha){
        linha.proguesso = 0;
        linha.ppm = 100 / calculoDeTempoDeProducaoDeUnidade(unidadeRecrutada);
        linha.terminada = false;
        linha.emEspera = false;
        linha.ativa = true;
        linha.unidadeSendoProduzida = unidadeRecrutada;
        StartCoroutine(linha.produzir());
        LinhasDeProducaoController.instancia.atualizarVisual(linha);
    }

    public void iniciarDesenvolvimento(Nacoes nacaoADesenvolver, LinhaDeProducao linha){    
        linha.proguesso = 0;
        linha.ppm = (100f / 120f * (nacaoADesenvolver.nivel+1f));
        linha.terminada = false;
        linha.emEspera = false;
        linha.ativa = true;
        linha.nacaoSendoDesenvolvida = nacaoADesenvolver;
        StartCoroutine(linha.produzir());
        LinhasDeProducaoController.instancia.atualizarVisual(linha);
    }

    public void despausarProducao(LinhaDeProducao linha){
        linha.ativa = true;
        linha.emEspera = false;
        StartCoroutine(linha.produzir());
    }

    public float calculoDeTempoDeProducaoDeUnidade(Unidade unidadeACalcular){
        int tempoAlevar = 0;//Quantos minutos vai terminar para produzir essa unidade
        if (unidadeACalcular == null) return 30;

        switch (unidadeACalcular.classe){
            case Classe.Infantaria:
            case Classe.Cavalaria:
                tempoAlevar += 20;
                break;

            case Classe.AntiTank:
            case Classe.Artilharia:
            case Classe.RocketArtilharia:
                tempoAlevar += 15;
                break;

            case Classe.TankDestroyer:
            case Classe.SPArtilharia:
            case Classe.SPRocketArtilharia:
                tempoAlevar += 60;
                break;

            case Classe.TanqueLeve: tempoAlevar += 30; break;
            case Classe.TanqueMedio: tempoAlevar += 60; break;
            case Classe.TanquePesado: tempoAlevar += 120; break;
            case Classe.TanqueUltraPesado: tempoAlevar += 180; break;

            case Classe.CAS:
            case Classe.Figthers:
            case Classe.Interceptors:
                tempoAlevar += 120;
                break;

            case Classe.SemiMotorizada: tempoAlevar += 30; break;
            case Classe.FullMotorizada: tempoAlevar += 60; break;
            case Classe.SemiMecanizada: tempoAlevar += 120; break;
            case Classe.FullMecanizada: tempoAlevar += 180; break;
        }
        return tempoAlevar *= unidadeACalcular.raridade;
    }

    public float calculoDeTempoDeConstrucaoDeEquipamento(Equipamento equipamentoConstruido)  {
        int tempoAlevar = 0;//Quantos minutos vai terminar para criar esse equipamento
        if (equipamentoConstruido == null) return 30;

        switch (equipamentoConstruido.tipoDeEquipamento) {
            case EquipamentoTipo.AntiTanque:
            case EquipamentoTipo.LancaChamas:
                tempoAlevar += 30;
                break;

            case EquipamentoTipo.Pistola:
            case EquipamentoTipo.Rifle:
                tempoAlevar += 15;
                break;

            case EquipamentoTipo.RifleDeAssalto:
                tempoAlevar += 45;
                break;

            case EquipamentoTipo.TanqueLeve: tempoAlevar += 40; break;
            case EquipamentoTipo.TanqueMedio: tempoAlevar += 60; break;
            case EquipamentoTipo.TanquePesado: tempoAlevar += 80; break;
            case EquipamentoTipo.TanqueUltraPesado: tempoAlevar += 100; break;

            case EquipamentoTipo.EquipamentosMotorizados: tempoAlevar += 40; break;
            case EquipamentoTipo.EquipamentosMecanizados: tempoAlevar += 60; break;

            case EquipamentoTipo.Artilharia: tempoAlevar += 40; break;
            case EquipamentoTipo.ArtilhariaFoguete: tempoAlevar += 40; break;

            case EquipamentoTipo.ArtilhariaSP:
            case EquipamentoTipo.AntiTanqueSP:
            case EquipamentoTipo.ArtilhariaFogueteSP:
                tempoAlevar += 80;
                break;


            case EquipamentoTipo.SubMetralhadora:
            case EquipamentoTipo.Metralhadora:
            case EquipamentoTipo.Escopeta:
                tempoAlevar += 30;
                break;
        }
        return tempoAlevar *= equipamentoConstruido.raridade;
    }

}
