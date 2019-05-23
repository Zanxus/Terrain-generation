using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxVeiwDistance = 450;
    public Transform veiwer;
    public Material mapMaterial;

    public static Vector2 veiwerPosition;
    static MapGenerator mapGenerator;
    int ChunkSize;
    int ChunksVisbleInVeiwDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> chunkVisibleLastUpdate = new List<TerrainChunk>();

    void Start(){
        mapGenerator = FindObjectOfType<MapGenerator>();
        ChunkSize = MapGenerator.mapChunkSize -1;
        ChunksVisbleInVeiwDistance = Mathf.RoundToInt(maxVeiwDistance / ChunkSize);
    }

    void Update() {
        veiwerPosition = new Vector2 (veiwer.position.x, veiwer.position.z);
        UpdateVisibleChunks();    
    }
    
    void UpdateVisibleChunks(){

        for (int i = 0; i < chunkVisibleLastUpdate.Count; i++){
            chunkVisibleLastUpdate[i].SetVisible(false);   
        }
        chunkVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(veiwerPosition.x / ChunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(veiwerPosition.y / ChunkSize);

        for (int yOffset = -ChunksVisbleInVeiwDistance; yOffset <= ChunksVisbleInVeiwDistance; yOffset++){
            for (int xOffset = -ChunksVisbleInVeiwDistance; xOffset <= ChunksVisbleInVeiwDistance; xOffset++){
                Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if(terrainChunkDictionary.ContainsKey (viewedChunkCoord)){
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisable()){
                        chunkVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                } else {
                    terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, ChunkSize, transform, mapMaterial));
                }
            }    
        }
    } 

    public class TerrainChunk {

        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 corrd, int size, Transform parent, Material material){
            position = corrd * size;
            bounds = new Bounds(position,Vector2.one*size);
            Vector3 positionV3 = new Vector3(position.x,0,position.y);

            meshObject = new GameObject("Terrain Chuck");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            
            SetVisible(false);
            
            mapGenerator.RequestMapData(OnMapDataReceived);

        }

        void OnMapDataReceived(MapData mapData) {
			mapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
		}
        void OnMeshDataReceived(MeshData MeshData){
            meshFilter.mesh = MeshData.CreateMesh();
        }

        public void UpdateTerrainChunk() {
           float veiwerDistanceFromNearestEgde = Mathf.Sqrt(bounds.SqrDistance (veiwerPosition));
           bool visable = veiwerDistanceFromNearestEgde <= maxVeiwDistance;
           SetVisible (visable);
        }

        public void SetVisible(bool visable){
            meshObject.SetActive (visable);
        }
        public bool IsVisable(){
            return meshObject.activeSelf;
        }
    }
}
