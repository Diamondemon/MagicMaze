using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    Grid grid = new Grid (48,48);
    List<Tile> tiles;
    List<Tile> tilesPile;
    
    // Start is called before the first frame update
    private void Start()
    {
        tiles = createTiles();
        tilesPile = tiles;
        placeFirstTile (22, 22, grid, tilesPile);
        generateText (grid);
    }

    // Update is called once per frame
    void Update()
    {
    }

    List<Tile> createTiles(){
        List<Tile> tiles = new List<Tile>();

        Square.squareType[,] startSquares = {{Square.squareType.TeleporterGreen, Square.squareType.TeleporterOrange, Square.squareType.OutPurple, Square.squareType.Timer}, {Square.squareType.OutYellow, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.OutOrange}, {Square.squareType.NoGo, Square.squareType.OutGreen, Square.squareType.TeleporterYellow, Square.squareType.TeleporterPurple}};
        Tile startTyle = new Tile (true, startSquares);
        tiles.Add(startTyle);

        Square.squareType[,] squares2 = {{Square.squareType.TeleporterGreen, Square.squareType.Normal, Square.squareType.OutOrange, Square.squareType.NoGo}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.NoGo, Square.squareType.NoGo}, {Square.squareType.TeleporterPurple, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}, {Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.NoGo, Square.squareType.Normal}};
        Tile tyle2 = new Tile (true, squares2);
        tiles.Add(tyle2);

        Square.squareType[,] squares3 = {{Square.squareType.NoGo, Square.squareType.Timer, Square.squareType.OutPurple, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal}, {Square.squareType.Normal, Square.squareType.Normal, Square.squareType.Normal, Square.squareType.TeleporterOrange}, {Square.squareType.NoGo, Square.squareType.OutYellow, Square.squareType.TeleporterGreen, Square.squareType.NoGo}};
        Tile tyle3 = new Tile (true, squares3);
        tiles.Add(tyle3);

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

