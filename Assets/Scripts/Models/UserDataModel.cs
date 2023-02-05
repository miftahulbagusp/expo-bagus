using System;
using UnityEngine;

[Serializable]

public struct UserDataModel
{
    public string Nickname { get; set; }
    public bool IsMale { get; set; }
    public float SkinColor { get; set; }
    public int FaceTypeID { get; set; }
    public int HairStyleID { get; set; }
    public float[] HairColor { get; set; }
    public int OutfitID { get; set; }
    public int AccessoriesGlassesID { get; set; }

    public UserDataModel(string nickname, bool isMale, int faceTypeID, int hairStyleID, float[] hairColor, int outfitID, int accessoriesGlassesID, float skinColor)
    {
        Nickname = nickname;
        IsMale = isMale;
        FaceTypeID = faceTypeID;
        HairStyleID = hairStyleID;
        HairColor = hairColor;
        OutfitID = outfitID;
        AccessoriesGlassesID = accessoriesGlassesID;
        SkinColor = skinColor;
    }
    
    public UserDataModel(string nickname, bool isMale, int faceTypeID, int hairStyleID, Color hairColor, int outfitID, int accessoriesGlassesID, float skinColor)
    {
        Nickname = nickname;
        IsMale = isMale;
        FaceTypeID = faceTypeID;
        HairStyleID = hairStyleID;
        HairColor = new []{hairColor.r, hairColor.g, hairColor.b};
        OutfitID = outfitID;
        AccessoriesGlassesID = accessoriesGlassesID;
        SkinColor = skinColor;
    }
}
