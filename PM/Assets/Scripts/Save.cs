using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class Save
{
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream newStream = new FileStream(Application.persistentDataPath + "/player.local", FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(newStream, player);
        newStream.Close();
    }

    public static int[] LoadPlayer()
    {
        if(File.Exists(Application.persistentDataPath + "/player.local"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream inStream = new FileStream(Application.persistentDataPath + "/player.local", FileMode.Open);

            PlayerData data = formatter.Deserialize(inStream) as PlayerData;
            //Or below since its just a cast
            //PlayerData data = (PlayerData)formatter.Deserialize(openStream);
            inStream.Close();
            return data.playerInfo;
        }

        return null;
    }
}

[Serializable]
public class PlayerData
{
    public int[] playerInfo;

    public PlayerData(Player player)
    {

    }
    //Save player info, once we determine the final stats
}
