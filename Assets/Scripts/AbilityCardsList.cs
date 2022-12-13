using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AbilityCardsNetworkList : NetworkVariableBase, INetworkSerializable, System.IEquatable<AbilityCardsNetworkList>
{
    // Start is called before the first frame update
    List<AbilityCard> cards;

    public AbilityCardsNetworkList(){
        cards = new List<AbilityCard>();
    }

    public void Add(AbilityCard card){
        cards.Add(card);
    }

    public AbilityCard At(int index){
        return cards[index];
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        AbilityCard[] array;

        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out length);
            array = new AbilityCard[length];
            for (int i=0; i<length; i++){
                reader.ReadNetworkSerializable(out array[i]);
            }
            cards.AddRange(array);
        }
        else
        {
            array = cards.ToArray();
            length = array.Length;
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(length);
            foreach (AbilityCard a in cards){
                writer.WriteNetworkSerializable(a);
            }
        }
    }

    public bool Equals(AbilityCardsNetworkList other)
    {
        return cards.TrueForAll((AbilityCard action)=>{return other.cards.Contains(action);});
    }

           public override void WriteField(FastBufferWriter writer)
       {
           // Serialize the data we need to synchronize
           writer.WriteValueSafe(SomeDataToSynchronize.Count);
           foreach (var dataEntry in SomeDataToSynchronize)
           {
               writer.WriteValueSafe(dataEntry.SomeIntData);
               writer.WriteValueSafe(dataEntry.SomeFloatData);
               writer.WriteValueSafe(dataEntry.SomeListOfValues.Count);
               foreach (var valueItem in dataEntry.SomeListOfValues)
               {
                   writer.WriteValueSafe(valueItem);
               }
           }
       }

       /// <summary>
       /// Reads the complete state from the reader and applies it
       /// </summary>
       /// <param name="reader">The stream to read the state from</param>
       public override void ReadField(FastBufferReader reader)
       {
           // De-Serialize the data being synchronized
           var itemsToUpdate = (int)0;
           reader.ReadValueSafe(out itemsToUpdate);
           SomeDataToSynchronize.Clear();
           for (int i = 0; i < itemsToUpdate; i++)
           {
               var newEntry = new SomeData();
               reader.ReadValueSafe(out newEntry.SomeIntData);
               reader.ReadValueSafe(out newEntry.SomeFloatData);
               var itemsCount = (int)0;
               var tempValue = (ulong)0;
               reader.ReadValueSafe(out itemsCount);
               newEntry.SomeListOfValues.Clear();
               for (int j = 0; j < itemsCount; j++)
               {
                   reader.ReadValueSafe(out tempValue);
                   newEntry.SomeListOfValues.Add(tempValue);
               }
               SomeDataToSynchronize.Add(newEntry);
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
