using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AbilityCardsNetworkList : NetworkVariableBase
{
    // Start is called before the first frame update
    public List<AbilityCard> cards;

    public AbilityCardsNetworkList(){
        cards = new List<AbilityCard>();
    }

    public void Add(AbilityCard card){
        cards.Add(card);
    }

    public AbilityCard At(int index){
        return cards[index];
    }

    public override void WriteField(FastBufferWriter writer)
    {
        // Serialize the data we need to synchronize
        writer.WriteValueSafe(cards.Count);
        foreach (AbilityCard a in cards)
        {
            writer.WriteNetworkSerializable(a);
        }
    }

    /// <summary>
    /// Reads the complete state from the reader and applies it
    /// </summary>
    /// <param name="reader">The stream to read the state from</param>
    public override void ReadField(FastBufferReader reader)
    {
        // De-Serialize the data being synchronized
        int itemsToUpdate = 0;
        reader.ReadValueSafe(out itemsToUpdate);
        cards.Clear();
        for (int i = 0; i < itemsToUpdate; i++)
        {
            var newEntry = new AbilityCard();
            reader.ReadNetworkSerializable(out newEntry);
            cards.Add(newEntry);
        }
    }

    public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    {
        // Do nothing for this example
    }

    public override void WriteDelta(FastBufferWriter writer)
    {
        // Do nothing for this example
    }

}
