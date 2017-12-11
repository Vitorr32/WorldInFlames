using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Logica.Classes;

public class BancoDeDados : MonoBehaviour {
    public static BancoDeDados instancia;
    //Aqui onde um id será convertido para uma Unidade ou Equipamento
    public List<Unidade> unidadesDoJogo = new List<Unidade>();//Essa lista inclue também os inimigos
    public Sprite[] visualPrincipalUnidadesDoJogo;
    public Sprite[] visualSecundarioUnidadesDoJogo;

    //public List<Construcao> construcoesDoJogo = new List<Construcao>();
    public List<Equipamento> equipamentosDoJogo = new List<Equipamento>();// Também inclue equipamento do inimigo
    public Sprite[] equipamentosDoJogoVisual;

    //Nacoes que o jogo possui, o jogador pode desbloquear elas mais tarde
    public List<Nacoes> nacoesDoJogo = new List<Nacoes>();
    //Campanhas que o jogo contém, conta com as restriuções, objetivos e etc.
    public List<Campanha> campanhasDoJogo = new List<Campanha>();

    //O banco de Dados e responsavel por criar um...banco de dados com todos os 
    //equipamentos e unidades para serem usadas, ele sempre estará pronto para fornece-los
    public Sprite[] iconesDeNacoes;
    public Sprite[] nomesDeNacoes;
    public Sprite[] iconesDasClasses;
    public Sprite[] iconesDeTiposDeEquipamentos = new Sprite[20];
    public Sprite[] iconeDosStatus;

    void Awake() {
        List<EquipamentoTipo> limitacoesDeInfantariaComum = new List<EquipamentoTipo>();
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanqueLeve);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanqueMedio);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanquePesado);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanqueUltraPesado);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.EquipamentosMotorizados);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.EquipamentosMecanizados);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.ArtilhariaSP);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.AntiTanqueSP);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.ArtilhariaFogueteSP);

        List<EquipamentoTipo> limitacoesDeCavalariaComum = new List<EquipamentoTipo>();
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanqueMedio);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanquePesado);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.TanqueUltraPesado);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.EquipamentosMecanizados);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.ArtilhariaSP);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.AntiTanqueSP);
        limitacoesDeInfantariaComum.Add(EquipamentoTipo.ArtilhariaFogueteSP);

        List<EquipamentoTipo> limitacoesDeTanquesAndMecanizadaComum = new List<EquipamentoTipo>();
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.Escopeta);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.Pistola);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.Rifle);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.RifleDeAssalto);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.Metralhadora);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.SubMetralhadora);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.AntiTanque);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.Artilharia);
        limitacoesDeTanquesAndMecanizadaComum.Add(EquipamentoTipo.ArtilhariaFoguete);

        List<EquipamentoTipo> limitacoesDeMotorizadosComum = new List<EquipamentoTipo>();
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.Escopeta);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.Pistola);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.Rifle);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.RifleDeAssalto);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.Metralhadora);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.SubMetralhadora);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.AntiTanque);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.Artilharia);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.ArtilhariaFoguete);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.EquipamentosMecanizados);
        limitacoesDeMotorizadosComum.Add(EquipamentoTipo.TanqueUltraPesado);

        instancia = this;
        //Coloque aqui os equipamentos do jogo

        //equipamentosDoJogo.Add(new Equipamento(1, "", "", 0, Pais.Alemanha, 2, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, 0, 3, 2, 10, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(0, "2.8 cm Panzerbüchse 41", "Uma arma Anti-Tanque produzida pela Alemanha em 1940 até 1943, pesando 229 kg e usando uma munição de 2.8cm ela foi usada principalmente por infantarias leves como tropas de Paraquedas e Alpinas.", 0, Pais.Alemanha, 3, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, -2, 4, 4, 16, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(1, "Panzerbüchse 38", "Os Rifle Anti-Tanques foram criados em 1917 pela Imperio Alemão em resposta aos Tanques Ingleses, a produção desses rifles foram retomadas na decada de 30 com o design de Gustloff Werke do Panzerbüchse 38, a arma tinha um complicado sistema de recuo fazia com que a arma emperrava diversas vezes em terrenos sujos", 0, Pais.Alemanha, 1, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, 0, 3, 1, 8, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(2, "Panzerbüchse 39", "Um redesign do Panzerbüchse 38 a versão 39 tinha pequenas mudanças de desing que focavam no aumento de cadência de fogo e recarregamento mais rapido, seu cano também ficou mais longo e seu peso menor", 1, Pais.Alemanha, 2, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, 0, 3, 2, 10, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(3, "Raketenpanzerbüchse 43", "Raketenpanzerbüchse 43 era um reusavel lançador de foguetes anti-tanque baseado na Bazooka usada pelos EUA, a arma ao atirar seu foguete emitia grandes quantidades de fumaça tanto na frente quanto atrás, o que exigia que as equipes que o utilizavam mudassem de posição assim que atirassem pois sua posição foi revelada e impedia seu uso dentro de ambientes fechados", 3, Pais.Alemanha, 3, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, 0, 0, 5, 15, 0, 0, -10, 0, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(4, "Raketenpanzerbüchse 54", "O sucessor do Raketenpanzerbüchse 43, a versão 54 contava com um protetor de explosões para proteção do usuario, seu alance efetivo foi aumentado para 180 e usava um novo foguete como munição melhorando seu dano, ele também pesava mais que seu antecessor, 12 kg, e um cano menor. Contudo ainda sofria dos problemas de ocultação do seu antecessor", 4, Pais.Alemanha, 4, EquipamentoTipo.AntiTanque, new int[] { 0, 0, 0, 0, 4, 6, 25, 0, 0, -10, 0, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(5, "Jagdpanther", "Jagdpanther foi um Destruidor de Tanques baseado no chassis do tanque Panther, por isso seu nome, dispunha de um poderoso canhão calibre 8.8cm dos tanques Tiger e a excelente armadura e suspensão do Panther, apesar de suas vantagens a Alemanha sofria de grande falta de materiais que resultou produção de pequena quantidade do destruidor de tanques", 4, Pais.Alemanha, 3, EquipamentoTipo.AntiTanqueSP, new int[] { 0, 0, 0, 23, 15, 6, 20, 0, 0, 0, 15, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(6, "Jagdpanzer IV", "Jagdpanzer IV foi um Destruidor de Tanques baseado no chassis do tanque Panzer IV, por isso seu nome, seu canhão principal tinha um calibre 7.5 cm e uma armadura de 80mm ", 4, Pais.Alemanha, 2, EquipamentoTipo.AntiTanqueSP, new int[] { 0, 0, 0, 16, 10, 3, 16, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(7, "Panzerjäger I", "Panzerjäger I foi o primeiro Destruidor de Tanques da Alemanha na Segunda Guerra Mundial, baseado no chassis do tanque Panzer I, seu canhão principal era o Checo Skoda 4.7cm e tinha uma armadura de 14.5mm", 0, Pais.Alemanha, 1, EquipamentoTipo.AntiTanqueSP, new int[] { 0, 0, 0, 20, 4, 2, 8, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(8, "Sturmgeschütz III Ausf.F", "O Sturmgeschütz III foi uma série de veiculos da Alemanha que focavam fogo direto envês de fogo de suporte, a variante F tinha um armamento principal maior que sua variante anterior e também uma armadura mais grossa, essa foi a mundaça que marcou a variante F mais como um Destruidor de Tanques que uma Artilharia de Fogo Direto", 4, Pais.Alemanha, 3, EquipamentoTipo.AntiTanqueSP, new int[] { 0, 0, 0, 40, 25, 10, 30, 5, 20, 0, 10, 20, 0, 0, 0, 0, 0 }));
        
        equipamentosDoJogo.Add(new Equipamento(9, "5 cm Granatwerfer 36", "O Granatwerfer Leve 36  foi um morteiro que começou a ser produzido em 1934, ele foi considerado complexo demais para seu proposito, tendo uma projetil fraco demais e um alance curto demais, além de ser dificil de controlar para sua equipe de 3 operadores", 0, Pais.Alemanha, 1, EquipamentoTipo.Artilharia, new int[] { 0, 0, 0, 0, 5, 8, 2, 0, -10, 0, 0, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(10, "8 cm Granatwerfer 34", "O Granatwerfer 34 foi um morteiro Alemão que foi considerado o morteiro padrão da infantaria durante toda a extensão da guerra, conhecido por sua rapida taxa de fogo e precisão, contudo sua efetividade era diretamente proporcional a experiencia de seus operadores", 0, Pais.Alemanha, 2, EquipamentoTipo.Artilharia, new int[] { 0, 0, 0, 0, 24, 14, 4, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(11, "Grille Ausf. H", "A serie Grille de veiculos foram artilharias automotorizadas produzidas pela Alemanha durante a guerra, a variante H usava o chassis do Panzer 38(t) e tinha o motor atrás, por isso a torre principal ficava na frente do veiculo", 4, Pais.Alemanha, 1, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 35, 47, 20, 8, 2, 0, 0, 12, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(12, "Grille Ausf. K", "A serie Grille de veiculos foram artilharias automotorizadas produzidas pela Alemanha durante a guerra, a variante K usava o chassis do Panzer III Ausf.M e tinha o motor na frente, permitindo a torre principal ficar na parte de trás do veiculo", 4, Pais.Alemanha, 1, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 35, 47, 20, 8, 2, 0, 0, 10, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(13, "Heuschrecke 10", "Heuschrecke 10 foi um proptotipo de artilharia automotorizada da Alemanha conceptualizado em 1944, a parte mais unica do seu design era que ele teria uma torre removivel que poderia ser utilizada como uma peça de artilharia, enquanto seus chassis poderia ser usado para carregar munição ou transportar infantaria", 5, Pais.Alemanha, 3, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 45, 12, 28, 12, 4, 0, 0, 10, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(14, "Hummel", "Hummel foi uma artilharia automotorizada da Alemanha visto serviço a partir de 1942, ele surgiu da nescessidade de suporte movel as divisões Panzers durante a invasão da URSS, Hummel usava o chassis de uma combinação dos melhores aspectos do Panzer III e IV e uma torre principal de 15cm", 4, Pais.Alemanha, 2, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 42, 133, 24, 10, 3, 0, 0, 5, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(15, "Sturmgeschütz III Ausf.E", "O Sturmgeschütz III foi uma série de veiculos da Alemanha que focavam fogo direto envês de fogo de suporte, a variante E teve a adição de uma caixa retangular para o equipamento de radio, esse espaço adicional permitia mais 6 munições para a torre principal, totalizando 50 projeteis, e uma metralhadora fixa, totalizando duas metralhadoras, para lidar com a infantaria inimiga", 3, Pais.Alemanha, 3, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 40, 25, 20, 20, 5, 20, 0, 10, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(16, "Wespe", "Wespe foi uma artilharia automotora da Alemanha produzida entre 1943 e 1944, durante a Batalha da França em 1940 o Panzer II se provou uma péssima escolha como tanque de batalha, por isso se procurou reusa-los, uma solução foi achada ao colocarem uma artilharia de campo de 10 cm, após grande sucesso da formula todos os chassis de Panzer II foram modificados para Wespes", 4, Pais.Alemanha, 3, EquipamentoTipo.ArtilhariaSP, new int[] { 0, 0, 0, 40, 106, 30, 10, 5, 0, 0, 5, 10, 0, 0, 0, 0, 0 }));
        
        equipamentosDoJogo.Add(new Equipamento(17, "Fallschirmjägergewehr 42", "A FG42 foi um rifle de batalha produzido pela Alemanha em 1942 até o final da guerra, o rifle era destinado para as divisões paraquedistas e por conta disso foram escassos como uma armamento de forças especiais, o design do rifle é considerado muito avançado para seu tempo e foi um predescessor para o primeiro rifle de assalto, o StG44", 4, Pais.Alemanha, 4, EquipamentoTipo.RifleDeAssalto, new int[] { 0, 0, 0, 0, 6, 50, 2, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(18, "Sturmgewehr 45", "O StG 45 foi o prototipo de um rifle de assalto Alemão no final da guerra, apesar de nunca ter saido do papel o rifle tinha um inovador sistema de recuo e de fogo seletivo, além de mais poderoso que seu predescessor o StG 44 ele era muito mais barato, custando apenas 45 Reichsmarks enquanto seu predescessor nescessitava de 70", 6, Pais.Alemanha, 5, EquipamentoTipo.RifleDeAssalto, new int[] { 0, 0, 0, 0, 8, 60, 3, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(19, "Sturmgewehr 44", "O StG 44 foi o primeiro rifle de assalto do mundo, suas inovações fizeram tanto para a industria de armas que até agora ele e considerado uma das maiores inovações desde a polvora sem fumaça, o rifle teve enorme sucesso no front Leste, apesar de ter vindo tarde demais para ter um efeito decisivo na guerra", 6, Pais.Alemanha, 5, EquipamentoTipo.RifleDeAssalto, new int[] { 0, 0, 0, 0, 6, 40, 3, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(20, "Sonderkraftfahrzeug 251", "Sd.Kfz. 251 foi, a partir de 1939, o veiculo de combate do seu tipo mais produzido na Alemanha, o veiculo tinha como proposito mover através de terreno dificeis de formar a transportar infantaria ao mesmo tempo que protegia ela com sua armadura e permitia o anexo de metralhadoras que proviam fogo de supressão, o Sd.Kfz. 251 teve varias variações e foi usado até o fim da guerra", 4, Pais.Alemanha, 1, EquipamentoTipo.EquipamentosMecanizados, new int[] { 0, 0, 0, 52, 0, 10, 10, 10, 0, 0, 15, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(21, "Flammenwerfer 35", "FmW 35 foi a primeira tentativa de se produzir um lança-chamas da Alemanha depois da Primeira Guerra Mundial, o FmW 35 podia projetar chamas até 30 metros de distancia do usuario, usado principalmente para limpar trincheiras e construções", 0, Pais.Alemanha, 2, EquipamentoTipo.LancaChamas, new int[] { 0, 0, 0, 0, 1, 10, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(22, "Flammenwerfer 41", "FmW 41 foi um pequeno redesign do FmW 35, seu sucessor pessava muito, 36kg, e era muito dificil de manusear, o que impedia operações rapidas o que exigiu um redesign, o resultado tinha um alcance de 32 metros e um peso de 28 kg", 2, Pais.Alemanha, 3, EquipamentoTipo.LancaChamas, new int[] { 0, 0, 0, 0, 1, 15, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(23, "Luger P08", "A pistola Luger P08 foi uma pistola produzida a partir de 1900 e usada em diversas nações, a pistola apesar de antiga contava com uma impressionante precisão da sua munição 19mm, a arma foi tão utilizada durante a Alemanha durante ambas guerras mundiais que ela se tornou um simbolo de oficiais alemães até hoje", 0, Pais.Alemanha, 3, EquipamentoTipo.Pistola, new int[] { 0, 0, 0, 0, 1, 10, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(24, "Mauser C96", "Construktion 96 é uma pistola semi-automatica produzida pela Alemanha, seu design contava com a munição a frente do gatilho é um cabo de madeira que lhe dava a estabilidade de um rifle de barril curto, também ouve produção de copias piratas da arma na Espanha e na China", 0, Pais.Alemanha, 1, EquipamentoTipo.Pistola, new int[] { 0, 0, 0, 0, 1, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(25, "Walther P38", "Walther P38 é uma pistola semi-automatica 9mm, ela foi desenvolvida em 1938 com o objetivo de substituir a Luger P08, arma foi produzida até a decada de 60, como pistola oficial da policia da Alemanha Ocidental", 1, Pais.Alemanha, 3, EquipamentoTipo.Pistola, new int[] { 0, 0, 0, 0, 1, 12, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(26, "Walther PPK", "Walther PPK era uma variação da Walther PP, significando Pistola Policial, a variante PPK era menor e mais facil de esconder, perfeita para espionagem", 0, Pais.Alemanha, 2, EquipamentoTipo.Pistola, new int[] { 0, 0, 0, 0, 1, 9, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(27, "King Tiger", "Tiger II foi um tanque pesado da Alemanha, uma variante do Tiger I ele foi criado para combater a nova gerações de tanques pesados e destruidores de tanques novos da URSS, tendo uma armadura e canhão principal maior, o que indica um aumento de peso e uma diminuição de mobilidade, o King Tiger conseguiu seu proposito, contudo sua perda de mobilidade o tornou menos poderoso fora da front Lest", 5, Pais.Alemanha, 4, EquipamentoTipo.TanquePesado, new int[] { 0, 0, 0, 41, 20, 20, 50, 20, 0, 0, 50, 40, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(28, "Tiger I", "O Tiger I foi um tanque pesado produzido pela Alemanha a partir de 1942, seu design foi considerado expecional para sua epoca ele era complicado demais, usando materiais caros e com varios problemas mecanicos em climas extremos ou terrenos barrosos e tinha uma alta taxa de consumo de combustivel, apesar de seus defeitos ainda era um extremamente efetivo e poderoso tanque de batalha" , 4, Pais.Alemanha, 4, EquipamentoTipo.TanquePesado, new int[] { 0, 0, 0, 45, 20, 20, 40, 20, 0, 0, 40, 40, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(29, "Panzerkampfwagen I", "Panzer I foi o primeiro design de tanques da Alemanha da decada de 30, o tanque level começou a ser produzido em 1934, o Panzer I viu serviço durante a Guerra Civil Espanhola em que serviu de introdução ao combate motorizado que iria resultar na doutrina Blitzkrieg anos depois, uma caracteristica do tanque e que ele não foi feita pra enfrentar outros tanques, sendo equipado somente com metralhadoras", 0, Pais.Alemanha, 1, EquipamentoTipo.TanqueLeve, new int[] { 0, 0, 0, 50, 20, 15, 2, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(30, "Panzerkampfwagen II", "O Panzer II foi produzido como um tanque temporario enquanto tanques mais avançados estavam sendo produzidos, o Panzer II era o mais numeroso tanque nos batalhões de tanques da Alemanha durante a invasão da Polônia e da França, após seu serviço seu chassis foi usado em varios outros tipos de veiculos", 0, Pais.Alemanha, 2, EquipamentoTipo.TanqueLeve, new int[] { 0, 0, 0, 40, 20, 15, 10, 0, 0, 0, 18, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(31, "Maschinengewehr 08", "MG 08 foi uma metralhadora produzida em 1908 pelo Imperio Alemão, uma adaptação da Maxim Gun britânica ela foi usada até o final da Segunda Guerra Mundial, contudo nesse periodo era considerado um armamento de segunda mão", 0, Pais.Alemanha, 1, EquipamentoTipo.Metralhadora, new int[] { 0, 0, 0, 0, 20, 15, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(32, "Maschinengewehr 15", "MG 15 foi uma metralhadora produzida na decada de 20 pela Republica de Weimar, durante a Segunda Guerra a arma foi usada em aviões da Luftwaffe para enfrentar outros aviões, com o avanço da tecnologia ela foi substituida mas achou usos no combate terrestre", 0, Pais.Alemanha, 2, EquipamentoTipo.Metralhadora, new int[] { 0, 0, 0, 0, 20, 18, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(33, "Maschinengewehr 42", "A MG 42 foi uma exceptional metralhadora Alemã produzida a partir de 1942 até os dias atuais, ela e reconhecida pela sua reliabilidade, durabilidade, simplicidade, e mais notavelmente o seu fogo supressivo, por isso ela foi usado extensivamente durante toda a duração da guerra", 4, Pais.Alemanha, 4, EquipamentoTipo.Metralhadora, new int[] { 0, 0, 0, 0, 47, 36, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(34, "Panther", "O Panther ou Panzerkampfwagen V foi um tanque médio produzido pela Alemanha a partir de 1943, com o objetivo de opor o T-34 sovietico, adotando a mesmsa armadura angulada dele, o Panther se provou um otimo tanque, tendo mais poder de fogo e armadura frontal que seu antecessor ainda mantendo boa mobilidade, contudo com perda de sua armadura lateral que o deixava vuneravel a fogo de flanco", 4, Pais.Alemanha, 3, EquipamentoTipo.TanqueMedio, new int[] { 0, 0, 0, 55, 22, 20, 35, 0, 0, 0, 30, 20, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(35, "Panzerkampfwagen III", "O Panzer III foi um tanque médio produzido pela Alemanha a partir de 1939, sua tarefa principal era enfrentar outros tanques enquanto suportava o mais pesado Panzer IV, contudo durante a Operação Barbarrosa ao enfrentarem o exception T-34 Sovietico os Alemães decidiram que o Panzer IV seria mais facil de ser modificado que o III, que foi ignorado a partir de então", 1, Pais.Alemanha, 1, EquipamentoTipo.TanqueMedio, new int[] { 0, 0, 0, 40, 15, 15, 25, 0, 0, 0, 23, 10, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(36, "Panzerkampfwagen IV", "O Panzer IV foi um tanque médio produzido pela Alemanha a partir de 1939, ele foi o único tanque Alemão que foi produzido durante todo o decorrer da queda sem parar, sofrendo pequenas alterações durante o periodo, além disso seu chassis foi usados para varios outras variantes de Destruidores de Tanques e Artilharia Automotora", 1, Pais.Alemanha, 2, EquipamentoTipo.TanqueMedio, new int[] { 0, 0, 0, 38, 15, 20, 35, 0, 0, 0, 27, 13, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(37, "SdKfz 231", "SdKfz 231 foi o primeiro carro blindado da Alemanha Nazista, ele servia com vários propositos incluindo reconhecimento, transporte e proteção de tropas menos resistentes, contava com uma tripulação de comandante, atirador, motorista e operador de radio", 0, Pais.Alemanha, 2, EquipamentoTipo.EquipamentosMotorizados, new int[] { 0, 0, 30, 85, 10, 12, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(38, "Gewehr 41(M)", "Gewehr 41(M) foi um rifle produzido pela Alemanha em 1941, em 1940 ficou aparente que os rifles precisavam de um grau de fogo mais elevado, foram feitos dois prototipos para tal rifle, um pela Mauser e outra pela Walther, o Gewehr 41(M), da Mauser, conseguiu seguir todas os criterios impostos pelo exercito para o rifle, contudo isso levou a um impreciso, pesado, complexo e travado rifle", 2, Pais.Alemanha, 2, EquipamentoTipo.Rifle, new int[] { 0, 0, 0, 0, 4, 4, 0, 0, -10, 0, 0, 3, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(39, "Gewehr 43", "Gewehr 43 foi um rifle produzido pela Alemanha em 1943, As falhas que foram seus atencessores Gewehr 41(M) e Gewehr 41(W) foram resultados diretos das exigências do exercito para as armas, o que acabava resultando em improvisos para respeitarem os criterios, contudo ao usar os caputrados SVT-40 da URSS os designers da Alemanha puderam finalmente fazer um design de sucesso para o rifle",4, Pais.Alemanha, 3, EquipamentoTipo.Rifle, new int[] { 0, 0, 0, 0, 8, 12, 0, 2, 0, 0, 0, 8, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(40, "Gewehr 98", "O rifle alemão Gewehr 98 era a arma padrão da infantaria do Imperio durante a Primeira Guerra Mundial, ele introduziu caracteristicas que seriam copiadas pelos seus rivais Britanicos e Americanos, sendo usado até 1947 até finalmente ser declarado obsoleto", 0, Pais.Alemanha, 1, EquipamentoTipo.Rifle, new int[] { 0, 0, 0, 0, 8, 6, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(41, "Karabiner 98k", "O rifle alemão Gewehr 98k foi um rifle de alta precisão produzido pela Alemanha a partir de 1935, a variante herdou do seu antecessor, o Gewehr 98, sua grande precisão, reliabilidade e alto alcance, o Karabiner 98k tinha um cano mais curto que seu antecessor o que exigiu um novo tipo de balas pois as do seu antecessor eram facéis de se detectar devido a seu flash", 0, Pais.Alemanha, 2, EquipamentoTipo.Rifle, new int[] { 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(42, "15 cm Nebelwerfer 41", "15 cm NbW 41 foi o primeiro lançador de foguetes Alemão produzido a partir de 1941, tinha o objetivo de lançar gases, fumaça ou projeteis explosivos a longas distancias, um atributo unico era que os foguetes tinham o propulsor na parte da frente e não de trás, supostamente melhorando a precisão, contudo isso complicava demais a produção do foguete com um efeito quase imperceptivel", 4, Pais.Alemanha, 1, EquipamentoTipo.ArtilhariaFoguete, new int[] { 0, 0, 0, 0, 22, 12, 12, 3, -20, -15, -15, 15, 0, 0, 0, 0, 0 }));
        
        equipamentosDoJogo.Add(new Equipamento(43, "21 cm Nebelwerfer 42", "21 cm NbW 42 era um lançador de foguetes Alemão produzido a partir de 1942, ele era capaz de lançar 42 foguetes, contudo seu exaustor não era efetivo, lançando até detritos nos seus proprios operadores, o que exigia que eles se escondessem antes de atirar, isso também levava a um recarregamento lento e serem facilmente descobertos por inimigos", 4, Pais.Alemanha, 2, EquipamentoTipo.ArtilhariaFoguete, new int[] { 0, 0, 0, 0, 78, 15, 15, 5, -10, -10, -35, 15, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(44, "Panzerwerfer", "Panzerwerfer foi um Lançador de Foguetes Automotor da Alemanha produzido a partir de 1943, lançadores de foguetes normalmente eram usados em situações que um bombardeamento era mais efetivo que fogo direto, nesse caso o número de detritos, barulhos, fumaça e estilhaços tinham um poderoso dano na moral do inimigo, o Panzerwerfer tinha 10 tubos e poderia carregar 20 projeteis", 4, Pais.Alemanha, 3, EquipamentoTipo.ArtilhariaFogueteSP, new int[] { 0, 0, 0, 0, 68, 25, 25, 5, -15, -5, -10, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(45, "Wurfrahmen 40", "Wurfrahmen 40 foi um Lançador de Foguetes Automotor da Alemanha produzido a partir de 1940, ele tinha como objetivo de servir como uma peça de artilharia mais movél, devido ao peso dos foguetes ele demorava para ser recarregado mas foi provado efetivo no suporte as divisões Panzer principalmente em aréas urbanas", 3, Pais.Alemanha, 2, EquipamentoTipo.ArtilhariaFogueteSP, new int[] { 0, 0, 0, 0, 45, 20, 20, 3, -18, -8, -15, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(46, "Maschinenpistole 18", "A MP18 foi a primeira efetiva metralhadora de mão do mundo, sendo produzida em 1918 pero Imperio Alemão, seu design iria influenciar todas as armas do seu tipo até 1960, contudo deveria se formar cuidado porque a arma tinha fama de atirar acidentalmente devido ao design", 0, Pais.Alemanha, 1, EquipamentoTipo.SubMetralhadora, new int[] { 0, 0, 0, 0, 2, 12, 2, 1, -15, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(47, "Maschinenpistole 38", "A MP18 foi uma metralhadora de mão da Alemanha produzida a partir de 1938, ela era uma simplificação da sua sucessora, a MP36, com foco em corte de custos para produção em massa, seu modelo serviria de base para a metralhadora de mão mais associadas as soldados alemães do periodo, a MP40", 1, Pais.Alemanha, 2, EquipamentoTipo.SubMetralhadora, new int[] { 0, 0, 0, 0, 2, 23, 4, 1, -15, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(48, "Maschinenpistole 40", "A MP18 foi uma marcante metralhadora de mão da Alemanha produzida a partir de 1940, ela era uma simplificação da sua sucessora, a MP38, com foco em corte de custos para produção em massa, como o foco em usar soldas em aonde podia, envês de ter cada parte da arma ser encaixada, a arma virou o padrão da infantaria Alemã durante os primeiros anos da guerra", 2, Pais.Alemanha, 3, EquipamentoTipo.SubMetralhadora, new int[] { 0, 0, 0, 0, 2, 30, 6, 6, -15, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(49, "Maschinenpistole 41", "A MP41 foi uma  metralhadora de mão da Alemanha produzida a partir de 1943, ela era uma pequena modificação da MP40, agora portando um estoque de madeira e um seletor de fogo, podendo mudar entre completamente automatico ou semi-automatico", 3, Pais.Alemanha, 3, EquipamentoTipo.SubMetralhadora, new int[] { 0, 0, 0, 0, 2, 33, 6, 6, -15, 0, 0, 0, 0, 0, 0, 0, 0 }));

        equipamentosDoJogo.Add(new Equipamento(50, "Maus", "Maus foi o prototipo de tanque ultra pesado Alemão, apenas um dos dois prototipos iniciais foram completados, a monstruosidade tinha 188 toneladas e não podia ser utilizado para cruzar pontes e é considerado até hoje o veiculo de combate pais pesado do mundo, contendo enorme poder de fogo e armadura impenetravel, Maus certamente seria um pessimo adversario numa batalha", 5, Pais.Alemanha, 5, EquipamentoTipo.TanqueUltraPesado, new int[] { 0, 0, 0, 20, 240, 20, 80, 30, 0, -70, 120, 90, 0, 0, 0, 0, 0 }));

        //Coloque aqui mais unidades para adicionar ao jogo
        //Unidades começam pelo ID 0

        //infantariaPadrao = new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 };
        //infantariaAlpina = new int[] { 23,23,5,4,8,15,3,2,80,40,35,40,10,100,25,100,15}
        //cavalariaPadrao = new int[]{25,25,6,14,6,12,1,1,65,40,20,30,10,100,20,100,15};
        //tanquePesadoPadrao = new int[]{50,50,4,18,14,30,45,15,90,30,70,70,60,100,60,100,40}
        //tanqueMedioPadrao = new int[]{40,40,4,30,10,20,25,3,85,45,50,50,40,100,40,100,30}
        //tanqueLevePadrao = new int[]{38,38,4,36,8,20,15,3,90,45,50,40,30,100,30,100,25}
        //infantariaMecanizada = new int[] {35,35,10,26,8,35,15,5,90,50,45,45,35,100,35,100,25}
        //infantariaMotorizada = new int[] {30,30,8,18,6,25,5,3,85,55,35,35,25,100,25,100,20}

        //1ºDivisão de Cavalaria Alemã, Mordeniza para 24º Divisão Panzer
        unidadesDoJogo.Add(new Unidade(0, 22, 1, 25, 0, "1º Divisão de Cavalaria", 1, 1, Classe.Cavalaria, Pais.Alemanha, Alvos.Macio,SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 5, 14, 5, 10, 2, 1, 70, 40, 20, 30, 20, 100, 20, 100, 5 }, 3, limitacoesDeCavalariaComum));
        //1º Divisão de Cavalaria Cossaca Alemã, Sem Mordenização to date
        unidadesDoJogo.Add(new Unidade(1, -1, 1, 0, 0, "1º Divisão de Cavalaria Cossaca", 1, 1, Classe.Cavalaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.CAVALARIA_MEDIEVAL, new int[] { 25, 25, 5, 14, 3, 15, 1, 1, 80, 30, 20, 50, 20, 100, 20, 100, 5 }, 3, limitacoesDeCavalariaComum));
        //7º Divisão Panzer "Divisão Fantasma", Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(2, -1, 1, 0, 0, "7º Divisão Panzer 'Divisão Fantasma'", 4, 3, Classe.TanqueMedio, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 55, 55, 15, 45, 12, 25, 30, 30, 120, 65, 60, 60, 75, 100, 75, 100, 50 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //4º Divisão de Infantaria Alemã, Mordeniza para a 14º Divisão Panzer
        unidadesDoJogo.Add(new Unidade(3, 21, 1, 30, 0, "4º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //9º Divisão de Infantaria Alemã, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(4, -1, 1, 30, 0, "9º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //10º Divisão de Infantaria Alemã, Mordeniza para 10º Divisão Motorizada
        unidadesDoJogo.Add(new Unidade(5, 24, 1, 30, 0, "10º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //23º Divisão de Infantaria Alemã, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(6, -1, 1, 30, 0, "23º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //33º Divisão de Infantaria Alemã, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(7, -1, 1, 30, 0, "33º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //60º Divisão de Infantaria Alemã, Mordeniza para 60º Divisão Semi-Motorizada
        unidadesDoJogo.Add(new Unidade(8, 17, 1, 40, 0, "60º Divisão de Infantaria", 1, 1, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 4, 6, 6, 10, 2, 1, 75, 30, 30, 40, 10, 100, 20, 100, 10 }, 4, limitacoesDeInfantariaComum));
        //Divisão de Infantaria "Bradenburgo" Alemã, Sem mordenização to Date
        unidadesDoJogo.Add(new Unidade(9, -1, 1, 30, 0, "Divisão de Infantaria 'Brandenburgo'", 3, 2, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 25, 25, 30, 6, 6, 15, 5, 10, 95, 45, 40, 50, 20, 100, 30, 100, 20 }, 4, limitacoesDeInfantariaComum));
        //6º Divisão SS de Infantaria Alpina Alemã, Sem mordenização to Date
        unidadesDoJogo.Add(new Unidade(10, -1, 1, 30, 0, "6º Divisão Alpina SS 'Nord'", 3, 2, Classe.InfantariaAlpina, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 27, 27, 6, 4, 8, 20, 5, 4, 85, 45, 40, 45, 20, 100, 35, 100, 25 }, 4, limitacoesDeInfantariaComum));
        //7º Divisão SS de Infantaria Alpina Alemã, Sem mordenização to Date
        unidadesDoJogo.Add(new Unidade(11, -1, 1, 30, 0, "7º Divisão Alpina SS 'Prinz Eugen'", 3, 2, Classe.InfantariaAlpina, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 27, 27, 6, 4, 8, 20, 5, 4, 85, 45, 40, 45, 20, 100, 35, 100, 25 }, 4, limitacoesDeInfantariaComum));
        //1º Divisão de Infantaria Alpina Alemã, Sem mordenização to Date
        unidadesDoJogo.Add(new Unidade(12, -1, 1, 30, 0, "1º Divisão Alpina", 1, 1, Classe.InfantariaAlpina, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 23, 23, 5, 4, 8, 15, 3, 2, 80, 40, 35, 40, 10, 100, 25, 100, 15 }, 4, limitacoesDeInfantariaComum));
        //Divisão de Infantaria Mecanizada Feldheernhalle, Mordeniza para Divisão Panzer Feldheernhalle 1
        unidadesDoJogo.Add(new Unidade(13, 23, 1, 85, 0, "Divisão de Infantaria Mecanizada 'Feldheernhalle'", 4, 4, Classe.FullMecanizada, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_SHOOTS, new int[] { 38, 38, 15, 30, 8, 40, 20, 12, 95, 60, 50, 55, 65, 100, 65, 100, 45 }, 3, limitacoesDeTanquesAndMecanizadaComum));
        //15º Divisão de Infantaria Motorizada Alemã, Mordeniza para 15º Divisão Panzer
        unidadesDoJogo.Add(new Unidade(14, 19, 1, 75, 0, "15º Divisão Motorizada", 2, 2, Classe.FullMotorizada, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_TANKS, new int[] { 30, 30, 8, 18, 6, 25, 5, 3, 85, 55, 35, 35, 25, 100, 25, 100, 20 }, 3, limitacoesDeMotorizadosComum));
        //Companhia de Bombardeio Marinho(Infantaria Naval) Alemã, Sem mordenização to Date
        unidadesDoJogo.Add(new Unidade(15, -1, 1, 30, 0, "Companhia de Bombardeio Marinho", 2, 1, Classe.InfantariaNaval, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 23, 23, 5, 4, 8, 15, 3, 2, 80, 40, 35, 40, 10, 100, 25, 100, 15 }, 4, limitacoesDeInfantariaComum));
        //60º Divisão de Infantaria Motorizada Alemã, Mordeniza para Divisão de Infantaria Mecanizada Feldheernhalle
        unidadesDoJogo.Add(new Unidade(16, 13, 1, 70, 0, "60º Divisão Motorizada", 3, 2, Classe.FullMotorizada, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 30, 30, 8, 18, 6, 25, 5, 3, 85, 55, 35, 35, 25, 100, 25, 100, 20 }, 3, limitacoesDeMotorizadosComum));
        //60º Divisão de Infantaria Semi-Motorizada Alemã, Mordeniza para 60º Divisão de Infantaria Motorizada Alemã
        unidadesDoJogo.Add(new Unidade(17, 16, 1, 60, 0, "60º Divisão de Infantaria Mordenizada", 2, 1, Classe.SemiMotorizada, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 28, 28, 7, 14, 6, 20, 3, 2, 85, 50, 30, 35, 20, 100, 25, 100, 20 }, 3, limitacoesDeInfantariaComum));
        //A Besta de Omaha, Sem mordenização(MAX)
        unidadesDoJogo.Add(new Unidade(18, -1, 1, 30, 0, "Heinrich Severloh 'A Besta da Praia de Omaha' ", 5, 4, Classe.Infantaria, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 50, 50, 15, 6, 6, 60, 20, 75, 120, 70, 40, 70, 10, 100, 100, 100, 20 }, 4, limitacoesDeInfantariaComum));
        //15º Divisão Panzer, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(19, -1, 1, 0, 0, "15º Divisão Panzer", 3, 2, Classe.TanqueLeve, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 38, 38, 4, 36, 8, 20, 15, 3, 90, 45, 50, 40, 30, 100, 30, 100, 25 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //12º Divisão SS Panzer Hitlerjugend, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(20, -1, 1, 0, 0, "12º Divisão Panzer SS 'Hitlerjugend'", 3, 2, Classe.TanqueMedio, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 42, 42, 5, 32, 10, 23, 28, 10, 85, 45, 50, 50, 45, 100, 45, 100, 35 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //14º Divisão Panzer, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(21, -1, 1, 0, 0, "14º Divisão Panzer", 3, 3, Classe.TanqueMedio, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 40, 40, 4, 30, 10, 20, 25, 3, 85, 45, 50, 50, 40, 100, 40, 100, 30 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //24º Divisão Panzer, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(22, -1, 1, 0, 0, "24º Divisão Panzer", 3, 3, Classe.TanqueMedio, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 40, 40, 4, 30, 10, 20, 25, 3, 85, 45, 50, 50, 40, 100, 40, 100, 30 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //Divisão Panzer Feldheernhalle 1, Sem mordenização to date
        unidadesDoJogo.Add(new Unidade(23, -1, 1, 0, 0, "Divisão Panzer Feldheernhalle 1", 5, 4, Classe.TanquePesado, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_TANKS, new int[] { 40, 40, 4, 30, 10, 20, 25, 3, 85, 45, 50, 50, 40, 100, 40, 100, 30 }, 2, limitacoesDeTanquesAndMecanizadaComum));
        //60º Divisão de Infantaria Motorizada Alemã, Mordeniza para Divisão de Infantaria Mecanizada Feldheernhalle
        unidadesDoJogo.Add(new Unidade(24, 25, 1, 25, 0, "10º Divisão Motorizada", 2, 2, Classe.FullMotorizada, Pais.Alemanha, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 30, 30, 8, 18, 6, 25, 5, 3, 85, 55, 35, 35, 25, 100, 25, 100, 20 }, 3, limitacoesDeMotorizadosComum));
        //Divisão de Infantaria Mecanizada Feldheernhalle, Mordeniza para Divisão Panzer Feldheernhalle 1
        unidadesDoJogo.Add(new Unidade(25, -1, 1, 85, 0, "10º Divisão de Infantaria Mecanizada", 3, 4, Classe.FullMecanizada, Pais.Alemanha, Alvos.Duro, SonsDeAtaque.WW2_SHOOTS, new int[] { 35, 35, 10, 26, 8, 35, 15, 5, 90, 50, 45, 45, 35, 100, 35, 100, 25 }, 3, limitacoesDeTanquesAndMecanizadaComum));

        //------------------------------------------------POLONIA-----------------------------------------------------
        //1ºDivisão de Cavalaria Polonesa, Sem mordenização
        unidadesDoJogo.Add(new Unidade(26, -1, 1, 25, 0, "1º Divisão de Cavalaria", 1, 1, Classe.Cavalaria, Pais.Polonia, Alvos.Macio, SonsDeAtaque.WW2_SHOOTS, new int[] { 20, 20, 5, 14, 5, 10, 2, 1, 70, 40, 20, 30, 20, 100, 20, 100, 5 }, 3, limitacoesDeCavalariaComum));

        //-------------------------------------------------FINLANDIA--------------------------------------------------
        //A Morte Branca, Sem mordenização(MAX)
        unidadesDoJogo.Add(new Unidade(27, -1, 1, 30, 0, "Simo Häyhä 'A Morte Branca'", 5, 4, Classe.Infantaria, Pais.Finlandia, Alvos.Macio, SonsDeAtaque.WW2_SNIPER_SHOOT, new int[] { 50, 50, 15, 6, 6, 60, 20, 75, 200, 70, 20, 80, 10, 100, 100, 100, 20 }, 4, limitacoesDeInfantariaComum));



        //-----------------------------------------------MISTFOLK----------------------------------------------------
        unidadesDoJogo.Add(new Unidade(28, -1, 1, 0, 0, "Cavalaria do Sacro Imperio", 1,0, Classe.Cavalaria,Pais.Nevoa, Alvos.Macio, SonsDeAtaque.CAVALARIA_MEDIEVAL, new int[] { 6, 6, 1, 14, 2, 6, 0, 0, 90, 20, 10, 25, 10, 100, 65, 100, 45 }, 3,limitacoesDeCavalariaComum));

        unidadesDoJogo.Add(new Unidade(29, -1, 1, 0, 0, "Infantaria Pesada do Sacro Imperio", 1, 0, Classe.Infantaria, Pais.Nevoa, Alvos.Macio, SonsDeAtaque.INFATARIA_MEDIEVAL, new int[] { 4, 4, 1, 3, 1, 5, 0, 0, 100, 15, 10, 25, 10, 100, 65, 100, 45 }, 3, limitacoesDeInfantariaComum));

        unidadesDoJogo.Add(new Unidade(30, -1, 1, 0, 0, "Milicia do Sacro Imperio", 1, 0, Classe.Infantaria, Pais.Nevoa, Alvos.Macio, SonsDeAtaque.INFATARIA_MEDIEVAL, new int[] { 3, 3, 1, 3, 1, 4, 0, 0, 80, 10, 5, 5, 10, 100, 65, 100, 45 }, 3, limitacoesDeInfantariaComum));

        unidadesDoJogo.Add(new Unidade(31, -1, 1, 0, 0, "Besteiros do Sacro Imperio", 1, 0, Classe.Infantaria, Pais.Nevoa, Alvos.Macio, SonsDeAtaque.CROSSBOW, new int[] { 3, 3, 1, 3, 4, 4, 0, 0, 80, 15, 10,10, 10, 100, 65, 100, 45 }, 3, limitacoesDeInfantariaComum));
 
        unidadesDoJogo.Add(new Unidade(32, -1, 1, 0, 0, "Cavalaria da Ortem Teutónica", 2, 0, Classe.Cavalaria, Pais.Nevoa, Alvos.Macio, SonsDeAtaque.CAVALARIA_MEDIEVAL, new int[] { 9, 9, 2, 14, 1, 8, 1, 1, 90, 25, 10, 25, 10, 100, 65, 100, 45 }, 3, limitacoesDeCavalariaComum));

        unidadesDoJogo.Add(new Unidade(33, -1, 1, 0, 0, "Infantaria da Ortem Teutónica", 1, 0, Classe.Infantaria, Pais.Nevoa, Alvos.Macio, SonsDeAtaque.INFATARIA_MEDIEVAL, new int[] { 6, 6, 2, 2, 1, 8, 1, 2, 100, 20, 10, 25, 10, 100, 65, 100, 45 }, 3, limitacoesDeInfantariaComum));

        unidadesDoJogo.Add(new Unidade(34, -1, 1, 0, 0, "Godofredo de Bulhão - Rei Cruzado", 1, 0, Classe.Infantaria, Pais.Nevoa, Alvos.Heroico, SonsDeAtaque.FOG_LORD, new int[] { 25, 25, 5, 6, 1, 15, 5, 5, 110, 40, 25, 35, 10, 100, 65, 100, 45 }, 3, limitacoesDeInfantariaComum));

        unidadesDoJogo.Add(new Unidade(35, -1, 1, 0, 0, "Trabuco do Sacro Imperio", 1, 0, Classe.Artilharia, Pais.Nevoa, Alvos.Macio,SonsDeAtaque.ARMA_DE_CERCO_MEDIEVAL, new int[] { 2, 2, 0, 2, 4, 10, 3, 0, 50, 0, 0, 0, 10, 100, 65, 100, 45 }, 3, null));

        //Aqui ficam as campanhas que o jogo possui
        string descricao =
            "28 de Agosto de 1939, há algums dias atrás o ministro das Relações Exteriores "+
            "Joachim von Ribbentrop enviou ao governo polonês o ultimato 'Danzig ou Guerra' " +
            "enquanto o governo Alemão espera a obvia rejeição da suas demandas e o começo da " +
            "guerra na Europa, o Major *Protagonista* comanda uma Guarnição no recem-adquirido " +
            "Sudetos da Checoslovaquia aguardando a hora em que ele possa comandar no front o " +
            "ambcioso oficial aguarda sua chance, sendo ensinado de perto por Heinz Guderian ele" +
            "absorveu os ensinamentos do novo estilo de guerra, que Guderian chamou de Blitzkrieg, "+
            "ao maximo.\n\n"+

            "Nesse dia em espefico contudo algo não-natural aconteceu, uma densa nevoa sobrevoou a "+
            "região, isso por si só não é tão incomun contudo a suposta nevoa apareceu em diversos "+
            "lugares do mundo em rapida sucessão e ficando cada vez mais densa até que antes do " +
            "meio-dia uma pessoa teria que se esforçar para perceber que não é noite " +

            "Finalmente as 14:00 horas a nevoa ,tão derepentemente quanto apareceu, desperçou-se "+
            "se perguntando o que sequer aconteceu o *Protagonista* volta a estudar os livros do "+
            "seu mentor até que um soldado entra na sua sala derepente e ofegante, sentido que "+
            "finalmente sua hora pode ter chegado ele pergunta ao soldado o motivo da pressa com "+
            "um assutador sorriso na cara, o soldado não entendendo bem a reação do seu comandante "+
            "da o seu relatorio.\n\n" +

            "- Uma estranha aglomeração de pessoas apareceu, provavelmente chegando aos trinta mil "+
            "elas estão vestidas de armaduras de placas de metais e os mais diversos tipos de armas "+
            "de combate corpo-a-corpo da idade média. \n\n"+

            "Que estranho o *Protagonista* pensou, mas não importa, provavelmente eram Checos fazendo "+
            "coisas estranhas como sempre, mas como eles estão armados e um motivo pra considera-los "+
            "uma ameaça mesmo que pequena, ele pergunta ao soldado pra onde esse 'exercito' se dirige. \n\n" +

            "- Berlin, senhor.\n\n"+

            "Vendo uma excelente oportunidade dessas *Protagonista* mobiliza sua guarnição e começa a "+
            "marchar.\n\n"+

            "Mal sabe ele, contudo, que esses inimigos são muito mais ameaçadores que aparentam";

        Limitacoes limitacoesDaCampanha = new Limitacoes();
        limitacoesDaCampanha.adicionarRestrincao(Classe.TanqueLeve,2);
        limitacoesDaCampanha.adicionarRestrincao(Classe.TanqueMedio,1);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TanquePesado);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TanqueUltraPesado);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.SPRocketArtilharia);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TankDestroyer);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.SPArtilharia);

        List<int[]> condicoesDeVitoria = new List<int[]>();
        condicoesDeVitoria.Add(new int[] { (int)Mundo0.Berlin });

        BancoDeDados.instancia.campanhasDoJogo.Add(new Campanha(0,"Berlin Schutz",Pais.Alemanha,Estacao.Verão,descricao, "World0-Berlin_Schutz", 12 ,condicoesDeVitoria,limitacoesDaCampanha));
        
        descricao =
            @"Seja lá o que foi as estranhas criaturas que tentaram atacar Berlin, elas foram derrotadas
            contudo aparições ainda maiores estão acontecendo no mundo, alguns dizem que Otto von Bismarck
            sai da sua catacumba em Friedrichsruh e comanda o exercito Prussiano de 1870, outros que 
            Frederico, O Grande, retorna com seu exercito da Guerra dos Sete Anos, o problema contudo e que
            a declaração de guerra a Polônia já foi feita e entregue e os dois Paises estão em estado de guerra

            Após a morte de um dos tenentes-coroneis que se preparavam para a Fall Weiss, a Invasão da Polônia,
            o *Protagonista* foi promovido e mandando imediatamente ao front como salvador de Berlin, satisfeito
            que apesar das dificuldades seu plano de auto-promoção funcionou ele então está pronto para mostrar ao
            mundo o Blitzkrieg na sua forma mais pura.....contudo uma mensagem interessante chegou da embaixada 
            Polonesa, aceitando sua ainda não completa mobilização, o fato que seus 'aliados' no Oeste estão com
            problemas internos e que a Wermatch está pronto para acabar com seu pais a Polonia volta atrás e decidiu
            aceitar o Ultimato por Danzig e o Corredor Polônes, que foi prontamente aceitado pelo ministro Ribbentrop.

            Assim a Invasão da Polonia que durou praticamente 3 horas foi um sucesso e Danzig voltou a Alemanha, mas é
            claro que isso não era o suficiente para as ambições de Hitler que exigiu uma relação de quase vassalagem a 
            Polonia, em troca das tropas polonesas estarem sobre o comando da Wermatch a Alemanha iria proteger a Polonia
            dos estranhos seres da nevoa e das ambições da União Sovietica, quebrando assim o celebre pacto Molotov-Ribbentrop

            Assim a Fall Weiss, uma operação de invasão se tornou uma de pacificação, as tropas alemãs iriam ocupar as cidades
            de Cracovia, Varsovia e Lvow enquanto 'recrutavam' tropas polonesas e preparavam para a possivel invasão da União
            Sovietica.

            Perdendo interessa na Polonia a Frente Leste se separou em dois Exercitos, um iria para a Frente Oeste para a 
            a preparação da Fall Rot e outro iria para a fronteira Polonesa-Sovietica para proteger o novo estado satelite
            enquanto pequenos batalhões iriam realmente pacificar as regiões, isso significa que a Alemanha estava praticamente
            deixando a Polonia a propria sorte contra os estranhos seres da nevoa

            Um pouco pertubado com o desenvolvimento o *Protagonista* recebeu sua missão, ele teria que pacificar uma serie
            de cidades polonesas até chegar em Danzig pelo sul, derrotando qualquer ser da nevoa que encontrar.

            Contudo certos relatorios indicam uma poderosa força da nevoa liderada por ninguem menos que John III Sobieski,
            que usous os lendarios Hussares Alados para defender Viena da Invasão Otomana seculos antes, estava se movendo
            rumo a Varsovia, o *Protagonista* então se pergunta, não seria muito mais interessante enfrentar tal lenda?";

        limitacoesDaCampanha = new Limitacoes();
        limitacoesDaCampanha.adicionarRestrincao(Classe.TanqueLeve, 3);
        limitacoesDaCampanha.adicionarRestrincao(Classe.TanqueMedio, 2);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TanquePesado);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TanqueUltraPesado);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.SPRocketArtilharia);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.TankDestroyer);
        limitacoesDaCampanha.adicionarClasseProibida(Classe.SPArtilharia);

        condicoesDeVitoria = new List<int[]>();
        condicoesDeVitoria.Add(new int[] { (int)Mundo1.Bremen });

        BancoDeDados.instancia.campanhasDoJogo.Add(new Campanha(1, "Crusader King",Pais.Alemanha,Estacao.Outono, descricao, "World1-CrusaderKing", 36,condicoesDeVitoria,limitacoesDaCampanha));
        

        BancoDeDados.instancia.campanhasDoJogo.Add(new Campanha(2, "Winged Hussars", Pais.Polonia, Estacao.Inverno, descricao, "CAMPANHA", 42, condicoesDeVitoria, limitacoesDaCampanha));
        

        BancoDeDados.instancia.campanhasDoJogo.Add(new Campanha(3, "Waterloo", Pais.Franca, Estacao.Inverno, descricao, "CAMPANHA", 42, condicoesDeVitoria, limitacoesDaCampanha));


        Nacoes novaNacao = new Nacoes();
        novaNacao.descricao = "O Terceiro Reich conta principalmente com unidades mecanizadas e tanques, aumentando grandemente os custos das suas unidades, mas as qualidades de suas tropas conseguem valer o investimento";
        novaNacao.pais = Pais.Alemanha;
        novaNacao.nivelLimite = 7;
        novaNacao.nome = "Terceiro Reich";
        novaNacao.modfCombustivel = 2.5f;
        novaNacao.modfMetal = 2.75f;
        novaNacao.modfMunicao = 0.75f;
        novaNacao.modfMaoDeObra = 0.5f;
        novaNacao.modfInspiracao = 1.0f;
        novaNacao.descricoesTecnologicas = new string[novaNacao.nivelLimite];
        novaNacao.descricoesTecnologicas[0] = "A Alemanha em 1936 contava com um exercito pequeno e fraco enquanto as ideias da Blitzkrieg estavam sendo construidas, eles contam com tanques leves e unidade de infantaria";
        novaNacao.descricoesTecnologicas[1] = "A Alemanha em 1939 tinha se militarizado rapidamente, contando agora com a poderosa Luftwaffe e um grande numero de unidades moveis e tanques de guerra leves e medios eles impõe uma projeção de poder nos campos de guerra da Europa";
        novaNacao.descricoesTecnologicas[2] = "A Alemanha em 1940 estava completamente mobilizada, depois dos ataques a Polonia,Dinamarca e Noruega ela finalmente iria enfrentar os seus inimigos da Primeira Guerra Mundial, contra a França a Alemanha dispunha de um crescente exercito com boa experiencia em combate";
        novaNacao.descricoesTecnologicas[3] = "A Alemanha em 1941 estava no topo do mundo, derrotando a França em semanas e deixando Londres em ruinas, ela volta sua atenção a União Sovietica, a batalha entre os dois Titãs europeus criou os primeiros tanques pesados e uma crescente mecanização das tropas terrestres";
        novaNacao.descricoesTecnologicas[4] = "A Alemanha em 1943 perdia muito da sua mão de obra, a desastrosa campanha do fronte leste estava fazendo a Alemanha pagar caro pelo erros estrategicos que fez na guerra contra a URSS, contudo nem tudo estava perdido, a experiencia com essas derrotas servira para criar novas armas e equipamentos, e também nascia os primeiros foguetes";
        novaNacao.descricoesTecnologicas[5] = "A Alemanha em 1944 apesar da maré ter se invertido, ainda dispunha de um exercito no topo de linha tecnologico, tenho unidades de tanque ultra pesadas e armas de ultima geração, contudo a perda constante de territorio e industrias acabou com as esperanças do Reich de mostrar ao mundo suas novas armas";
        novaNacao.descricoesTecnologicas[6] = "A Alemanha em 1945 assinava seu termo de rendição, os ultimos meses da guerra na Europa viram o poderoso exercito mecanizado alemão ser substituido por milicias com equipamentro pobre, contudo ainda se mostrava que a maquina de guerra Alemã ainda dispunha de novas armas, apesar de elas não terem conseguido mudar o invevitavel";
        nacoesDoJogo.Add(novaNacao);


        novaNacao = new Nacoes();
        novaNacao.descricao = "Polonia é um pais que so recentemente adiquiriu sua independencia e ainda tem muito para provar suas unidades são básicas e baratas, mas ainda pode-se achar herois entre os Poloneses";
        novaNacao.pais = Pais.Polonia;
        novaNacao.nivelLimite = 3;
        novaNacao.nome = "Republica Polonesa";
        novaNacao.modfCombustivel = 0.5f;
        novaNacao.modfMetal = 0.5f;
        novaNacao.modfMunicao = 0.5f;
        novaNacao.modfMaoDeObra = 0.5f;
        novaNacao.modfInspiracao = 1.0f;
        novaNacao.descricoesTecnologicas = new string[novaNacao.nivelLimite];
        novaNacao.descricoesTecnologicas[0] = "A Polonia em 1936 está finalmente se desenvolvendo contudo os seus expansionistas vizinhos a deixam em alerta, até agora a Polonia conta com unidades de infantaria basica e cavalaria";
        novaNacao.descricoesTecnologicas[1] = "A Polonia em 1939 recebeu o ultimato 'Danzig ou Guerra' prontamente rejeitando e contando com a ajuda dos Aliados ao oeste ela entra em guerra com a nova superpotencia militar do mundo, nessa epoca a Polonia ja podia contar com unidades de tanque leve e uma infantaria mais especializada";
        novaNacao.descricoesTecnologicas[2] = "A Polonia em 1940 ja tinha se rendindo, porem sua liberdade nunca será tomada, varias milicias na polonia e pilotos que se juntaram a Royal Air Force Inglesa prova que os Poloneses ainda estão dispostos a lutar";

    }

    public Unidade retornarUnidade(int id){
        foreach(Unidade u in unidadesDoJogo){
            if(u.idNoBancoDeDados == id){
                return ClonagemProfunda<Unidade>(u);
            }
        }
        return null;
    }

    public Equipamento retornarEquipamento(int id){
        foreach (Equipamento e in equipamentosDoJogo){
            if (e.id == id){                
                return ClonagemProfunda<Equipamento>(e);
            }
        }
        return null;
    }

    public Nacoes retornarNacao(Pais pais){
        foreach (Nacoes e in nacoesDoJogo){
            if (e.pais == pais){
                return ClonagemProfunda<Nacoes>(e);
            }
        }
        return null;
    }

    /*public Construcao retornarConstrucao(int id){
        foreach (Construcao c in construcoesDoJogo){
            if (c.id == id){                
                return ClonagemProfunda<Construcao>(c);
            }
        }
        return null;
    }*/

    public static T ClonagemProfunda<T>(T obj){
        using (var ms = new MemoryStream()){
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    public string tellMeThisClassName(Classe classe){
        switch (classe){
            case Classe.Infantaria: return "Infantaria Classica";
            case Classe.InfantariaNaval: return "Fuzileiro Naval";
            case Classe.InfantariaAlpina: return "Infantaria Alpina";
            case Classe.Artilharia: return "Artilharia";
            case Classe.AntiTank: return "Antitanque";
            case Classe.RocketArtilharia: return "Artilharia de Foguetes";
            case Classe.Cavalaria: return "Infantaria Montada";
            case Classe.FullMecanizada: return "Infantaria Mecanizada";
            case Classe.FullMotorizada: return "Infantaria Motorizada";
            case Classe.SemiMecanizada: return "Infantaria Semi-Mecanizada";
            case Classe.SemiMotorizada: return "Infantaria Semi-Motorizada";
            case Classe.TankDestroyer: return "Antitanque automotora";
            case Classe.SPArtilharia: return "Artilharia automotora";
            case Classe.SPRocketArtilharia: return "Artilharia de Foguetes automotora";
            case Classe.TanqueLeve: return "Tanque Leve";
            case Classe.TanqueMedio: return "Tanque Medio";
            case Classe.TanquePesado: return "Tanque Pesado";
            case Classe.TanqueUltraPesado: return "Tanque Ultra Pesado";
            default: return null;
        }
    }

}
