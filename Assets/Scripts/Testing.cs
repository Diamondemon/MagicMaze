using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script principal du jeu, à attache sur n'importe quel object du jeu (par exemple un Empty qui ne sert qu'à ça)


public class Testing : MonoBehaviour
{
    Grid grid = new Grid (48,48);
    List<Tile> tiles;
    List<Tile> tilesPile;
    

    // Start is called before the first frame update
    private void Start()
    {
        AbilityCard abCardTest = new AbilityCard(moveUp:true);
        PawnController pawn = new PawnController();

        tiles = createTiles();
        // Creation des tuiles correspondant aux tuiles du jeu
        tilesPile = tiles;
        tilesPile = placeFirstTile (22, 22, grid, tilesPile);
        grid = extendMaze(23, 22, grid, tilesPile);
        tilesPile.RemoveAt(0);
        grid = extendMaze(22, 24, grid, tilesPile);
        tilesPile.RemoveAt(0);
        grid = extendMaze(24, 25, grid, tilesPile);
        tilesPile.RemoveAt(0);
        grid = extendMaze(25, 23, grid, tilesPile);
        tilesPile.RemoveAt(0);

//  generateText utilisé pour debugger
        generateText (grid);

        pawn.currentPosition = grid.GetSquare(22,22);
        abCardTest.ShowPossibleAction(pawn);
    }

    // Update is called once per frame
    void Update()
    {
    }

    List<Tile> createTiles(){
        List<Tile> tiles = new List<Tile>();

        Square.squareType[,] startSquares = {{Square.squareType.TeleporterGreen, Square.squareType.TeleporterOrange, Square.squareType.OutPurple, Square.squareType.Timer}, {Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.TeleporterYellow, Square.squareType.TeleporterPurple}};
        Tile startTyle = new Tile (true, startSquares, "Plane", grid);
        tiles.Add(startTyle);

        Square.squareType[,] squares2 = {{Square.squareType.TeleporterGreen, Square.squareType.Normal, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}};
        Tile tyle2 = new Tile (false, squares2, "Plane.002", grid);
        tiles.Add(tyle2);

        Square.squareType[,] squares3 = {{Square.squareType.NoGo, Square.squareType.Timer, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.OutYellow, Square.squareType.TeleporterGreen, Square.squareType.NoGo}};
        Tile tyle3 = new Tile (false, squares3, "Plane.003", grid);
        tiles.Add(tyle3);

        Square.squareType[,] squares4 = {{Square.squareType.NoGo, Square.squareType.TeleporterOrange, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.NoGo}, {Square.squareType.NoGo, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutPurple}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.NoGo, Square.squareType.TeleporterYellow}};
        Tile tyle4 = new Tile (false, squares4, "Plane.004", grid);
        tiles.Add(tyle4);

        Square.squareType[,] squares5 = {{Square.squareType.NoGo, Square.squareType.TeleporterPurple, Square.squareType.OutYellow, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Timer, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.Normal, Square.squareType.Normal}};
        Tile tyle5 = new Tile (false, squares5, "Plane.005", grid);
        tiles.Add(tyle5);

        return tiles;
    }

    List<Tile> placeFirstTile(int x, int y, Grid grid, List<Tile> tilePile){
        for (int i=0; i<4; i++){
            for (int j=0; j<4; j++){
                grid.gridArray[x+i,y+j] = tilePile[0].squares[i,j];
            }
        }
        tilePile.RemoveAt(0);
        return tilePile;
    }

//Ajoute la tuile à la grille du jeu (équivalent de poser une tuile dans la vraie vie)
    Grid addTileToGrid(int x, int y, Grid grid, Tile tile){
        for (int i=0; i<4; i++){
            for (int j=0; j<4; j++){
                grid.gridArray[x+i,y+j] = tile.squares[i,j];
            }
        }
        return grid;
    }

//A lancer quand un pion arrive sur une loupe de sa couleur pour poser la première tuile de la pile dans la bonne orientation à l'endroit voulu
    Grid extendMaze(int x, int y, Grid grid, List<Tile> tilePile){
        Tile newTile = tilePile[0];
        GameObject tileMesh = GameObject.Find(newTile.meshName);
        Square currentSquare = grid.gridArray[x,y];
        Vector3[] neighbourCoordinates = {new Vector3 (x+1,0,y), new Vector3 (x,0,y+1), new Vector3 (x-1,0,y), new Vector3 (x,0,y-1)};

        foreach (Vector3 coordinate in neighbourCoordinates){
            if (grid.gridArray[(int)coordinate[0], (int)coordinate[2]]==null){
                if (coordinate[0]==x & coordinate[2]==y+1){
                    tileMesh.transform.position = new Vector3(coordinate[0]+1, 0, coordinate[2]+2);
                    grid = addTileToGrid((int) coordinate[0]-1, (int) coordinate[2], grid, newTile);
                }
                if (coordinate[0]==x+1 & coordinate[2]==y){
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0]+2, 0, coordinate[2]);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0], (int) coordinate[2]-2, grid, newTile);
                }
                if (coordinate[0]==x & coordinate[2]==y-1){
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0], 0, coordinate[2]-1);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0]-2, (int) coordinate[2]-3, grid, newTile);
                }
                if (coordinate[0]==x-1 & coordinate[2]==y){
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    newTile = rotateTile(newTile);
                    tileMesh.transform.position = new Vector3(coordinate[0]-1, 0, coordinate[2]+1);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    tileMesh.transform.Rotate(0, 0, 90);
                    grid = addTileToGrid((int) coordinate[0]-3, (int) coordinate[2]-1, grid, newTile);
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
                newSquares[i,j] = new Square (tile.squares[3-j,i].type, 3-j, i);
            }
        }
        Tile newTile = new Tile (false, newSquares, tile.meshName);
        return newTile;
    }

//génère des couleurs en fonction du type des cases 
    void generateText (Grid grid){
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
}

