using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = System.Random;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private UIManager uiMgr;
    [SerializeField] public GameObject Bleu;
    [SerializeField] public GameObject Rouge;
    [SerializeField] public GameObject Jaune;
    [SerializeField] public GameObject Vert;

    [SerializeField] private Material goOverlayMaterial;
    [SerializeField] private Material transparent;

    [SerializeField] private PlayerController localPlayer;

    private Camera currentCamera;

    Grid grid = new Grid (48,48);

    GameObject[,] tilesOverlay;
    private Vector2Int currentHover;

    List<Tile> tiles;
    List<Tile> tilesPile;
    List<int> tilesPileIndices;

    AbilityCardsNetworkList cards;

    PawnController pionVert;
    PawnController pionRouge;
    PawnController pionJaune;
    PawnController pionBleu;

    [SerializeField] public PawnController currentPawn;

    bool isMovingPiece;
    List<Vector2Int> allowedDestinations;

    private Vector2Int toMovePosition = new Vector2Int();

    private bool escapePressed = false;

    private void Awake() {
        tiles = createTiles();
        tilesPile = tiles;
        placeFirstTile (22, 22);

        generateTileOverlay ();

        createAbilityCards();

        allowedDestinations = new List<Vector2Int>();
        
        isMovingPiece = false;
        initializePawnControllers();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer){
            Server_ShuffleAndTell();
            tilesPileIndices = new List<int>();
            for (int i=0; i<tilesPile.Count;i++){
                tilesPileIndices.Add(i);
            }
            shuffle(tilesPileIndices);
        }
    }

    private void Server_ShuffleAndTell(){
        shuffle(cards.cards);
        cards.SetDirty(true);
        assignPlayerCardClientRpc();
    }


    // Start is called before the first frame update
    void Start()
    {

        spawnPawns();
        #if UNITY_EDITOR
        if (NetworkManager.Singleton == null){
            Debug.Log("Pas de Network manager actif.");
            return;
        }
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient){
            NetworkManager.Singleton.StartHost();
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)){
            if (!escapePressed) {
                escapePressed = true;
                uiMgr.ToggleEscapeMenu();
            }
        }
        else if (escapePressed) escapePressed = false;

        if (!currentCamera){
            currentCamera=Camera.current;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover"))){
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            if (Input.GetMouseButtonDown(0)){
                cleanOverlay();
                if (isPawnHere(hitPosition.x, hitPosition.y)){
                    currentPawn = pawnHere(hitPosition.x, hitPosition.y);
                    if (!currentPawn.isSelected.Value){
                        currentPawn.ToggleSelectPawnServerRpc(true);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)){
                if(isMovingPiece == true) {
                    toMovePosition = hitPosition;
                    currentPawn.ToggleSelectPawnServerRpc(false);
                }
            }
        }
    }

    [ClientRpc]
    public void TogglePawnSelectionClientRpc(bool selected, ClientRpcParams clientRpcParams = default){
        if (selected){
            isMovingPiece = true;
            allowedDestinations = ShowPossibleAction(localPlayer, currentPawn);
        }
        else {
            if (allowedDestinations.Contains(toMovePosition)){
                currentPawn.MoveToServerRpc(toMovePosition.x, toMovePosition.y);
                if (checkForExploration(currentPawn, toMovePosition.x, toMovePosition.y)){
                    extendMazeServerRpc(toMovePosition.x, toMovePosition.y);
                }
            }
            allowedDestinations = new List<Vector2Int>();
            cleanOverlay();
            isMovingPiece = false;
            currentPawn = null;
        }
    }

    private void createAbilityCards(){
        cards = new AbilityCardsNetworkList();
        cards.Add(new AbilityCard(false, true, true, false));
        cards.Add(new AbilityCard(true, false, false, false));
        cards.Add(new AbilityCard(false, false, false, true));
    }

    [ClientRpc]
    private void assignPlayerCardClientRpc(ClientRpcParams clientRpcParams = default){
        localPlayer.abilityCard = cards.At((int) NetworkManager.Singleton.LocalClientId);

    }

    private void initializePawnControllers(){
        pionBleu = Bleu.GetComponent<PawnController>();
        pionBleu.grid = grid;
        pionBleu.gameManager = this;
        pionJaune = Jaune.GetComponent<PawnController>();
        pionJaune.grid = grid;
        pionJaune.gameManager = this;
        pionVert = Vert.GetComponent<PawnController>();
        pionVert.grid = grid;
        pionVert.gameManager = this;
        pionRouge = Rouge.GetComponent<PawnController>();
        pionRouge.grid = grid;
        pionRouge.gameManager = this;
    }

    private void spawnPawns(){
        pionBleu.MoveToServerRpc(23,23);
        pionJaune.MoveToServerRpc(23,24);
        pionVert.MoveToServerRpc(24,23);
        pionRouge.MoveToServerRpc(24,24);
    }

    private List<Tile> createTiles(){
        List<Tile> tiles = new List<Tile>();

        Square.squareType[,] startSquares = {{Square.squareType.TeleporterGreen, Square.squareType.TeleporterOrange, Square.squareType.OutPurple, Square.squareType.Timer}, {Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.TeleporterYellow, Square.squareType.TeleporterPurple}};
        Tile startTyle = new Tile (true, startSquares, "Plane");
        startTyle.addwall(0,0,0,1);
        startTyle.addwall(0,1,0,2);
        startTyle.addwall(0,2,0,3);
        startTyle.addwall(3,1,3,2);
        startTyle.addwall(3,2,3,3);
        tiles.Add(startTyle);

        Square.squareType[,] squares2 = {{Square.squareType.TeleporterGreen, Square.squareType.Normal, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}};
        Tile tyle2 = new Tile (false, squares2, "Plane.002");
        tyle2.addwall(0,0,1,0);
        tiles.Add(tyle2);

        Square.squareType[,] squares3 = {{Square.squareType.NoGo, Square.squareType.Timer, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.OutYellow, Square.squareType.TeleporterGreen, Square.squareType.NoGo}};
        Tile tyle3 = new Tile (false, squares3, "Plane.003");
        tyle3.addwall(0,1,0,2);
        tyle3.addwall(0,2,1,2);
        tyle3.addwall(1,1,1,2);
        tyle3.addwall(2,1,2,2);
        tyle3.addwall(3,1,3,2);
        tyle3.addwall(1,3,2,3);
        tiles.Add(tyle3);

        Square.squareType[,] squares4 = {{Square.squareType.NoGo, Square.squareType.TeleporterOrange, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutPurple}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.NoGo, Square.squareType.TeleporterYellow}};
        Tile tyle4 = new Tile (false, squares4, "Plane.004");
        tyle4.addwall(1,2,2,2);
        tiles.Add(tyle4);

        Square.squareType[,] squares5 = {{Square.squareType.NoGo, Square.squareType.TeleporterPurple, Square.squareType.OutYellow, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle5 = new Tile (false, squares5, "Plane.005");
        tyle5.addwall(0,1,0,2);
        tyle5.addwall(0,2,1,2);
        tyle5.addwall(1,2,2,2);
        tyle5.addwall(2,2,3,2);
        tyle5.addwall(2,2,2,3);
        tyle5.addwall(1,0,1,1);
        tiles.Add(tyle5);

        Square.squareType[,] squares6 = {{Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutGreen}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}};
        Tile tyle6 = new Tile (false, squares6, "Plane.006");
        tyle6.addwall(1,1,1,2);
        tiles.Add(tyle6);

        Square.squareType[,] squares7 = {{Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.TeleporterOrange, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle7 = new Tile (false, squares7, "Plane.007");
        tyle7.addwall(2,2,2,3);
        tiles.Add(tyle7);

        Square.squareType[,] squares8 = {{Square.squareType.TeleporterGreen, Square.squareType.NoGo, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}};
        Tile tyle8 = new Tile (false, squares8, "Plane.008");
        tyle8.addwall(0,2,0,3);
        tiles.Add(tyle8);

        Square.squareType[,] squares9 = {{Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.TeleporterGreen}, {Square.squareType.TeleporterYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}};
        Tile tyle9 = new Tile (false, squares9, "Plane.009");
        tiles.Add(tyle9);

        Square.squareType[,] squares10 = {{Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.OutOrange, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.TeleporterYellow, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.OutPurple, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle10 = new Tile (false, squares10, "Plane.010");
        tyle10.addwall(2,2,3,2);
        tiles.Add(tyle10);

        return tiles;
    }

    void placeFirstTile(int x, int y){
        for (int i=0; i<4; i++){
            for (int j=0; j<4; j++){
                grid.gridArray[x+i,y+j] = tilesPile[0].squares[i,j];
            }
        }
        tilesPile.RemoveAt(0);
    }

    List<T> shuffle<T>(List<T> list){
        var rng = new Random();
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T element = list[k];  
            list[k] = list[n];  
            list[n] = element;  
        }
        return list;
    }

//Ajoute la tuile à la grille du jeu (équivalent de poser une tuile dans la vraie vie)
    Grid addTileToGrid(int x, int y, Tile tile){
        for (int i=0; i<4; i++){
            for (int j=0; j<4; j++){
                grid.gridArray[x+i,y+j] = tile.squares[i,j];
            }
        }
        return grid;
    }

//A lancer quand un pion arrive sur une loupe de sa couleur pour poser la première tuile de la pile dans la bonne orientation à l'endroit voulu
    bool checkForExploration(PawnController pawn, int x, int y){
        if (grid.gridArray[x,y].type==Square.squareType.OutGreen){
           if (pawn.color==PawnController.Color.green){
                return true;
            }
        }
        if (grid.gridArray[x,y].type==Square.squareType.OutOrange){
           if (pawn.color==PawnController.Color.red){
                return true;
            }
        }
        if (grid.gridArray[x,y].type==Square.squareType.OutPurple){
           if (pawn.color==PawnController.Color.blue){
                return true;
            }
        }
        if (grid.gridArray[x,y].type==Square.squareType.OutYellow){
           if (pawn.color==PawnController.Color.yellow){
                return true;
            }
        }
        return false;
    }

    [ServerRpc(RequireOwnership=false)]
    void extendMazeServerRpc(int x, int y){
        int index = tilesPileIndices[0];
        extendMazeClientRpc(x, y, index);
        tilesPileIndices.RemoveAt(0);
    }
    
    [ClientRpc]
    void extendMazeClientRpc(int x, int y, int index){
        Tile newTile = tilesPile[index];
        GameObject tileMesh = GameObject.Find(newTile.meshName);
        Square currentSquare = grid.gridArray[x,y];
        Vector3[] neighbourCoordinates = {new Vector3 (x+1,0,y), new Vector3 (x,0,y+1), new Vector3 (x-1,0,y), new Vector3 (x,0,y-1)};

        foreach (Vector3 coordinate in neighbourCoordinates){
            if (grid.gridArray[(int)coordinate[0], (int)coordinate[2]]==null){
                //if explore top
                if (coordinate[0]==x & coordinate[2]==y+1){
                    tileMesh.transform.position = new Vector3(coordinate[0]+1, 0, coordinate[2]+2);
                    grid = addTileToGrid((int) coordinate[0]-1, (int) coordinate[2], newTile);

                    grid.gridArray[x,y].up = grid.gridArray[x,y+1];
                    grid.gridArray[x,y+1].down = grid.gridArray[x,y];
                }
                if (coordinate[0]==x+1 & coordinate[2]==y){
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0]+2, 0, coordinate[2]);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0], (int) coordinate[2]-2, newTile);

                    grid.gridArray[x,y].right = grid.gridArray[x+1,y];
                    grid.gridArray[x+1,y].left = grid.gridArray[x,y];
                }
                if (coordinate[0]==x & coordinate[2]==y-1){
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0], 0, coordinate[2]-1);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0]-2, (int) coordinate[2]-3, newTile);

                    grid.gridArray[x,y].down = grid.gridArray[x,y-1];
                    grid.gridArray[x,y-1].up = grid.gridArray[x,y];
                }
                if (coordinate[0]==x-1 & coordinate[2]==y){
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0]-1, 0, coordinate[2]+1);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0]-3, (int) coordinate[2]-1, newTile);

                    grid.gridArray[x,y].left = grid.gridArray[x-1,y];
                    grid.gridArray[x-1,y].right = grid.gridArray[x,y];
                }
            }
        }
        //return grid;
    } 

//tourne une tuile 
    Tile rotateTile (Tile tile){
        Square[,] newSquares = new Square [4,4];
        for (int i=0;i<4;i++){
            for (int j=0;j<4;j++){
                newSquares[i,j] = new Square (tile.squares[3-j,i].type);
                newSquares[i,j].up = tile.squares[3-j,i].left;
                newSquares[i,j].right = tile.squares[3-j,i].up;
                newSquares[i,j].down = tile.squares[3-j,i].right;
                newSquares[i,j].left = tile.squares[3-j,i].down;
            }
            newSquares[i,3].up = new Square(Square.squareType.NoGo);
            newSquares[i,0].down = new Square(Square.squareType.NoGo);
            newSquares[0,i].left = new Square(Square.squareType.NoGo);
        }

        Tile newTile = new Tile (false, newSquares, tile.meshName);
        return newTile;
    }

    void generateTileOverlay (){
        tilesOverlay = new GameObject[48,48];
        for (int x=0; x<48; x++){
            for (int y=0; y<48; y++){
                GameObject tileOverlay = new GameObject(string.Format("X:{0}, Y:{1}", x, y));

                Mesh mesh = new Mesh();
                tileOverlay.AddComponent<MeshFilter>().mesh = mesh;
                tileOverlay.AddComponent<MeshRenderer>().material = transparent;

                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(x,0.1f,y);
                vertices[1] = new Vector3(x,0.1f,y+1);
                vertices[2] = new Vector3(x+1,0.1f,y);
                vertices[3] = new Vector3(x+1,0.1f,y+1);

                int[] tris = new int[] {0,1,2,1,3,2};
                mesh.vertices = vertices;
                mesh.triangles = tris;

                tileOverlay.AddComponent<BoxCollider>();
                tileOverlay.layer = LayerMask.NameToLayer("Tile");
                tilesOverlay[x,y]=tileOverlay;
            }
        }
    }

    GameObject createText(string text, Vector2 position, Color color){
        GameObject gameObject = new GameObject("Tile_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        Vector3 position3D = new Vector3(position.x+0.5f, 0.5f, position.y+0.5f);
        transform.localPosition = position3D;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = color;
        textMesh.fontSize = 4;
        return gameObject;
    }

    //Select operations
    private Vector2Int LookupTileIndex(GameObject hitInfo){
        for (int x=0; x<48;x++){
            for (int y=0; y<48 ;y++){
                if (tilesOverlay[x,y] == hitInfo){
                    return  new Vector2Int(x,y);
                }
            }
        }
        return -Vector2Int.one; //-1,-1 should not happen
    }

    private void showOverlay(int x, int y){
        tilesOverlay[x,y].GetComponent<MeshRenderer>().material = goOverlayMaterial;
    }
    private void hideOverlay(int x, int y){
        tilesOverlay[x,y].GetComponent<MeshRenderer>().material = transparent;
    }
    private void cleanOverlay(){
        for (int x=0;x<48;x++){
            for (int y=0;y<48;y++){
                hideOverlay(x,y);
                }
            }
    }
    bool isPawnHere(int x, int y){
        if (pionBleu.x.Value==x & pionBleu.y.Value==y){
            return true;
        }
        if (pionVert.x.Value==x & pionVert.y.Value==y){
            return true;
        }
        if (pionJaune.x.Value==x & pionJaune.y.Value==y){
            return true;
        }
        if (pionRouge.x.Value==x & pionRouge.y.Value==y){
            return true;
        }
        else{
            return false;
        }
    }
    private PawnController pawnHere(int x, int y){
        if (pionBleu.x.Value==x & pionBleu.y.Value==y){
            return pionBleu;
        }
        if (pionVert.x.Value==x & pionVert.y.Value==y){
            return pionVert;
        }
        if (pionJaune.x.Value==x & pionJaune.y.Value==y){
            return pionJaune;
        }
        if (pionRouge.x.Value==x & pionRouge.y.Value==y){
            return pionRouge;
        }
        else {
            return pionBleu;
        }
    }

    public List<Vector2Int> ShowPossibleAction(PlayerController player, PawnController pawn)
    {
        Square square = pawn.currentPosition;
        Square squareStart = pawn.currentPosition;
        AbilityCard abilityCard = player.abilityCard;
        int x = pawn.x.Value;
        int y = pawn.y.Value;
        int x0 = x;
        int y0 = y;

        List<Vector2Int> liste = new List<Vector2Int>();

        foreach (AbilityCard.Action a in abilityCard.actions)
        {
            if(a == AbilityCard.Action.escalator || a == AbilityCard.Action.explore || a == AbilityCard.Action.teleport)
            {
                //On est pas cense passer par la, vu qu'on utilise pas ces actions 

            } else {

                // On se contente des cas ou les actions sont "bouger dans une des 4 directions"
                // soit moveLeft, moveRight, moveUp, moveDown
                while(square.type != Square.squareType.NoGo)
                {
                    showOverlay(x, y);
                    liste.Add(new Vector2Int(x,y));
                    if (a==AbilityCard.Action.moveUp){
                        y+=1;
                    }
                    if (a==AbilityCard.Action.moveDown){
                        y-=1;
                    }
                    if (a==AbilityCard.Action.moveRight){
                        x+=1;
                    }
                    if (a==AbilityCard.Action.moveLeft){
                        x-=1;
                    }
                    square = GetNextSquare(a, square, x, y);
                }
            }
            square = squareStart;
            x=x0;
            y=y0;
        }
        return liste;
    }

    Square GetNextSquare(AbilityCard.Action a, Square s, int nextX, int nextY){

        if (isPawnHere(nextX, nextY)){
            return new Square(Square.squareType.NoGo);
        }

        switch (a){
            case AbilityCard.Action.moveUp:
                return s.up;
            case AbilityCard.Action.moveRight:
                return s.right;
            case AbilityCard.Action.moveDown:
                return s.down;
            case AbilityCard.Action.moveLeft:
                return s.left;
            default:
                return new Square(Square.squareType.NoGo);
        }
    }
}


