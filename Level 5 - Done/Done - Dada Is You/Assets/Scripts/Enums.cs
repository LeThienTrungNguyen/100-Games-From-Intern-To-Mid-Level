using UnityEngine;

public enum ObjectType
{
    // Real objects
    DADA,
    ROCK,
    WALL,
    FLAG,
    WATER,
    SKULL,
    LAVA,
    GRASS,
    
    // Text objects
    TEXT_DADA,
    TEXT_ROCK,
    TEXT_WALL,
    TEXT_FLAG,
    TEXT_IS,
    TEXT_YOU,
    TEXT_STOP,
    TEXT_PUSH,
    TEXT_WIN,
    TEXT_SINK,
    TEXT_DEFEAT,
    TEXT_WATER,
    TEXT_SKULL,
    TEXT_LAVA,
    TEXT_HOT,
    TEXT_MELT,
    TEXT_GRASS
}

public enum WordType
{
    NOUN,
    OPERATOR,
    PROPERTY
}

public enum Property
{
    YOU,
    STOP,
    PUSH,
    WIN,
    SINK,
    DEFEAT,
    HOT,
    MELT
}
