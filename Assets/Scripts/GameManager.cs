using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiMgr;
    [SerializeField] public GameObject Bleu;
    [SerializeField] public GameObject Rouge;
    [SerializeField] public GameObject Jaune;
    [SerializeField] public GameObject Vert;

    [SerializeField] private Material goOverlayMaterial;
    [SerializeField] private Material transparent;

    private Camera currentCamera;

    Grid grid = new Grid (48,48);

    GameObject[,] tilesOverlay;
    private Vector2Int currentHover;

    List<Tile> tiles;
    List<Tile> tilesPile;

    PlayerController player1;
    PlayerController player2;
    PlayerController player3;

    AbilityCard card1;
    AbilityCard card2;
    AbilityCard card3;

    PawnController pionVert;
    PawnController pionRouge;
    PawnController pionJaune;
    PawnController pionBleu;

    [SerializeField] public PawnController currentPawn;
    PlayerController currentPlayer;

    bool isMovingPiece;
    List<Vector2Int> allowedDestinations;

    // Start is called before the first frame update
    void Start()
    {
        tiles = createTiles();
        tilesPile = tiles;
        tilesPile = placeFirstTile (22, 22, tilesPile);
        tilesPile = shuffle(tilesPile);

        generateTileOverlay ();
        generateText (grid);

        createAbilityCards();
        createPlayers();

        isMovingPiece = false;

        spawnPawns();
        currentPlayer = player1;

        #if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsHost){
            NetworkManager.Singleton.StartHost();
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)){
            uiMgr.DisplayEscapeMenu();
        }
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
                    isMovingPiece = true;
                    currentPawn = pawnHere(hitPosition.x, hitPosition.y);
                    allowedDestinations = ShowPossibleAction(currentPlayer, currentPawn);
                    Debug.Log(allowedDestinations[0]);
                }
            }

            if (Input.GetMouseButtonUp(0)){
                if(isMovingPiece == true) {
                    if (allowedDestinations.Contains(hitPosition)){
                        currentPawn.moveTo(hitPosition.x, hitPosition.y, grid);
                        if (checkForExploration(currentPawn, hitPosition.x, hitPosition.y)){
                            grid = extendMaze(hitPosition.x, hitPosition.y, grid, tilesPile);
                            tilesPile.RemoveAt(0);
                        }
                    }
                    allowedDestinations = new List<Vector2Int>();
                    cleanOverlay();
                }
            }
        }
    }

    private void createAbilityCards(){
        card1 = new AbilityCard(false, true, true, false);
        card2 = new AbilityCard(true, false, false, false);
        card3 = new AbilityCard(false, false, false, true);
    }

    private void createPlayers(){
        player1 = new PlayerController(card1);
        player2 = new PlayerController(card2);
        player3 = new PlayerController(card3);
    }

    private void spawnPawns(){
        pionBleu = Bleu.GetComponent<PawnController>();
        pionBleu.moveTo(23,23, grid);
        pionJaune = Jaune.GetComponent<PawnController>();
        pionJaune.moveTo(23,24, grid);
        pionVert = Vert.GetComponent<PawnController>();
        pionVert.moveTo(24,23,grid);
        pionRouge = Rouge.GetComponent<PawnController>();
        pionRouge.moveTo(24,24, grid);
    }

    private List<Tile> createTiles(){
        List<Tile> tiles = new List<Tile>();

        Square.squareType[,] startSquares = {{Square.squareType.TeleporterGreen, Square.squareType.TeleporterOrange, Square.squareType.OutPurple, Square.squareType.Timer}, {Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.TeleporterYellow, Square.squareType.TeleporterPurple}};
        Tile startTyle = new Tile (true, startSquares, "Plane");
        tiles.Add(startTyle);

        Square.squareType[,] squares2 = {{Square.squareType.TeleporterGreen, Square.squareType.Normal, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}};
        Tile tyle2 = new Tile (false, squares2, "Plane.002");
        tiles.Add(tyle2);

        Square.squareType[,] squares3 = {{Square.squareType.NoGo, Square.squareType.Timer, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.OutYellow, Square.squareType.TeleporterGreen, Square.squareType.NoGo}};
        Tile tyle3 = new Tile (false, squares3, "Plane.003");
        tiles.Add(tyle3);

        Square.squareType[,] squares4 = {{Square.squareType.NoGo, Square.squareType.TeleporterOrange, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutPurple}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.NoGo, Square.squareType.TeleporterYellow}};
        Tile tyle4 = new Tile (false, squares4, "Plane.004");
        tiles.Add(tyle4);

        Square.squareType[,] squares5 = {{Square.squareType.NoGo, Square.squareType.TeleporterPurple, Square.squareType.OutYellow, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle5 = new Tile (false, squares5, "Plane.005");
        tiles.Add(tyle5);

        Square.squareType[,] squares6 = {{Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutGreen}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}};
        Tile tyle6 = new Tile (false, squares6, "Plane.006");
        tiles.Add(tyle6);

        Square.squareType[,] squares7 = {{Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.TeleporterOrange, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle7 = new Tile (false, squares7, "Plane.007");
        tiles.Add(tyle7);

        Square.squareType[,] squares8 = {{Square.squareType.TeleporterGreen, Square.squareType.NoGo, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}};
        Tile tyle8 = new Tile (false, squares8, "Plane.008");
        tiles.Add(tyle8);

        Square.squareType[,] squares9 = {{Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.TeleporterGreen}, {Square.squareType.TeleporterYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo}};
        Tile tyle9 = new Tile (false, squares9, "Plane.009");
        tiles.Add(tyle9);

        Square.squareType[,] squares10 = {{Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.OutOrange, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.TeleporterYellow, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.OutPurple, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle10 = new Tile (false, squares10, "Plane.010");
        tiles.Add(tyle10);

        return tiles;
    }

    List<Tile> placeFirstTile(int x, int y, List<Tile> tilePile){
        for (int i=0; i<4; i++){
            for (int j=0; j<4; j++){
                grid.gridArray[x+i,y+j] = tilePile[0].squares[i,j];
            }
        }
        tilePile.RemoveAt(0);
        return tilePile;
    }

    List<Tile> shuffle(List<Tile> tiles){
        var rng = new Random();
        int n = tiles.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Tile tile = tiles[k];  
            tiles[k] = tiles[n];  
            tiles[n] = tile;  
        }
        return tiles;
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
    
    Grid extendMaze(int x, int y, Grid grid, List<Tile> tilePile){
        Tile newTile = tilePile[0];
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
        return grid;
    } 

//tourne une tuile 
    Tile rotateTile (Tile tile){
        Square[,] newSquares = new Square [4,4];
        for (int i=0;i<4;i++){
            for (int j=0;j<4;j++){
                newSquares[i,j] = new Square (tile.squares[3-j,i].type);
            }
        }

        for (int i=0; i<4; i++){
            newSquares[i,3].up = new Square(Square.squareType.NoGo);
            newSquares[i,3].down = newSquares[i,2];
            newSquares[i,2].up = newSquares[i,3];
            newSquares[i,2].down = newSquares[i,1];
            newSquares[i,1].up = newSquares[i,2];
            newSquares[i,1].down = newSquares[i,0];
            newSquares[i,0].up = newSquares[i,1];
            newSquares[i,0].down = new Square(Square.squareType.NoGo);

            newSquares[0,i].left = new Square(Square.squareType.NoGo);
            newSquares[0,i].right = newSquares[1,i];
            newSquares[1,i].left = newSquares[0,i];
            newSquares[1,i].right = newSquares[2,i];
            newSquares[2,i].left = newSquares[1,i];
            newSquares[2,i].right = newSquares[3,i];
            newSquares[3,i].left = newSquares[2,i];
            newSquares[3,i].right = new Square(Square.squareType.NoGo);
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

//génère des couleurs en fonction du type des cases 
    void generateText (Grid grid){
        Debug.Log("go");
        for (int x=0; x<grid.gridArray.GetLength(0);x++){
            for (int y=0; y<grid.gridArray.GetLength(1); y++){
                if (grid.gridArray[x,y] != null) {
                    if (grid.gridArray[x,y].type == Square.squareType.NoGo){
                        createText("0", new Vector2(x,y), Color.black);
                    }
                    else if (grid.gridArray[x,y].type == Square.squareType.OutPurple){
                        createText("0", new Vector2(x,y), Color.blue);
                    }
                    else if (grid.gridArray[x,y].type == Square.squareType.OutGreen){
                        createText("0", new Vector2(x,y), Color.green);
                    }
                    else if (grid.gridArray[x,y].type == Square.squareType.OutYellow){
                        createText("0", new Vector2(x,y), Color.yellow);
                    }
                    else if (grid.gridArray[x,y].type == Square.squareType.OutOrange){
                        createText("0", new Vector2(x,y), Color.red);
                    }
                    else{
                        createText("0", new Vector2(x,y), Color.white);
                    }
                }
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
        if (pionBleu.x==x & pionBleu.y==y){
            return true;
        }
        if (pionVert.x==x & pionVert.y==y){
            return true;
        }
        if (pionJaune.x==x & pionJaune.y==y){
            return true;
        }
        if (pionRouge.x==x & pionRouge.y==y){
            return true;
        }
        else{
            return false;
        }
    }
    private PawnController pawnHere(int x, int y){
        if (pionBleu.x==x & pionBleu.y==y){
            return pionBleu;
        }
        if (pionVert.x==x & pionVert.y==y){
            return pionVert;
        }
        if (pionJaune.x==x & pionJaune.y==y){
            return pionJaune;
        }
        if (pionRouge.x==x & pionRouge.y==y){
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
        int x = pawn.x;
        int y = pawn.y;
        int x0 = x;
        int y0 = y;

        List<Vector2Int> liste = new List<Vector2Int>();

        foreach (AbilityCard.action a in abilityCard.actions)
        {
            if(a == AbilityCard.action.escalator || a == AbilityCard.action.explore || a == AbilityCard.action.teleport)
            {
                //On est pas cense passer par la, vu qu'on utilise pas ces actions 

            } else {

                // On se contente des cas ou les actions sont "bouger dans une des 4 directions"
                // soit moveLeft, moveRight, moveUp, moveDown
                while(square.type != Square.squareType.NoGo)
                {
                    showOverlay(x, y);
                    liste.Add(new Vector2Int(x,y));
                    square = GetNextSquareByAction(a, square);
                    if (a==AbilityCard.action.moveUp){
                        y+=1;
                    }
                    if (a==AbilityCard.action.moveDown){
                        y-=1;
                    }
                    if (a==AbilityCard.action.moveRight){
                        x+=1;
                    }
                    if (a==AbilityCard.action.moveLeft){
                        x-=1;
                    }
                }
            }
            square = squareStart;
            x=x0;
            y=y0;
        }
        return liste;
    }

    Square GetNextSquareByAction(AbilityCard.action a, Square s){
        if (a==AbilityCard.action.moveUp){
            return s.up;
        }
        if (a==AbilityCard.action.moveRight){
            return s.right;
        }
        if (a==AbilityCard.action.moveDown){
            return s.down;
        }
        if (a==AbilityCard.action.moveLeft){
            return s.left;
        }
        else{
            return new Square(Square.squareType.NoGo);;
        }
    }
}


