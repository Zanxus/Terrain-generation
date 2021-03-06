﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset){
        float[,] NoiseMap = new float[mapWidth,mapHeight];

        System.Random prng =new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2 [octaves];
        for (int i = 0; i < octaves; i++){ 
            float offSetX = prng.Next(-100000,100000) + offset.x;
            float offSetY = prng.Next(-100000,100000) + offset.y;
            octavesOffsets[i] = new Vector2(offSetX,offSetY);    
        }

        if(scale <= 0){
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2;
        float halfHeight = mapHeight;

        for (int y = 0; y< mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++){
                    float sampleX = (x - halfWidth) /scale * frequency + octavesOffsets[i].x;
                    float sampleY = (y - halfHeight) /scale * frequency + octavesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY) * 2 -1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight){
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight){
                    minNoiseHeight = noiseHeight;
                    }

                NoiseMap[x,y] = noiseHeight;
            }
            
        } 
        for (int y = 0; y< mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                NoiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, NoiseMap[x,y]);
            }
        }

        return NoiseMap;
    }
}
