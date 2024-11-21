using fNbt;
using fNbt.Tags;
using Newtonsoft.Json;

namespace NbtParser;

public static class NbtParser
{
    public static string? ParseToJson(NbtCompound? nbtCompound)
    {
        if (nbtCompound == null) return null;
        var dictionary = ConvertNbtToDictionary(nbtCompound);
        return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
    }

    private static Dictionary<string, object> ConvertNbtToDictionary(NbtCompound? nbtCompound)
    {
        if (nbtCompound == null) return [];
        var result = new Dictionary<string, object>();
        foreach (var tag in nbtCompound.Tags)
        {
            switch (tag)
            {
                case NbtByte nbtByte:
                    if (tag.Name != null) result[tag.Name] = nbtByte.ByteValue;
                    break;
                case NbtShort nbtShort:
                    result[tag.Name] = nbtShort.ShortValue;
                    break;
                case NbtInt nbtInt:
                    result[tag.Name] = nbtInt.IntValue;
                    break;
                case NbtLong nbtLong:
                    result[tag.Name] = nbtLong.LongValue;
                    break;
                case NbtFloat nbtFloat:
                    result[tag.Name] = nbtFloat.FloatValue;
                    break;
                case NbtDouble nbtDouble:
                    result[tag.Name] = nbtDouble.DoubleValue;
                    break;
                case NbtString nbtString:
                    result[tag.Name] = nbtString.StringValue;
                    break;
                case NbtByteArray nbtByteArray:
                    result[tag.Name] = nbtByteArray.ByteArrayValue;
                    break;
                case NbtIntArray nbtIntArray:
                    result[tag.Name] = nbtIntArray.IntArrayValue;
                    break;
                case NbtLongArray nbtLongArray:
                    result[tag.Name] = nbtLongArray.LongArrayValue;
                    break;
                case NbtList nbtList:
                    result[tag.Name] = ConvertNbtListToArray(nbtList);
                    break;
                case NbtCompound subCompound:
                    result[tag.Name] = ConvertNbtToDictionary(subCompound);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported NBT type: {tag.TagType}");
            }
        }

        return result;
    }

    private static List<object> ConvertNbtListToArray(NbtList nbtList)
    {
        var result = new List<object>();
        foreach (var tag in nbtList)
        {
            switch (tag)
            {
                case NbtByte nbtByte:
                    result.Add(nbtByte.ByteValue);
                    break;
                case NbtShort nbtShort:
                    result.Add(nbtShort.ShortValue);
                    break;
                case NbtInt nbtInt:
                    result.Add(nbtInt.IntValue);
                    break;
                case NbtLong nbtLong:
                    result.Add(nbtLong.LongValue);
                    break;
                case NbtFloat nbtFloat:
                    result.Add(nbtFloat.FloatValue);
                    break;
                case NbtDouble nbtDouble:
                    result.Add(nbtDouble.DoubleValue);
                    break;
                case NbtString nbtString:
                    result.Add(nbtString.StringValue);
                    break;
                case NbtCompound subCompound:
                    result.Add(ConvertNbtToDictionary(subCompound));
                    break;
                case NbtList subList:
                    result.Add(ConvertNbtListToArray(subList));
                    break;
                default:
                    throw new NotSupportedException($"Unsupported NBT type in list: {tag.TagType}");
            }
        }

        return result;
    }

    public static NbtCompound? ParseFromJson(string? json)
    {
        if (json == null) return null;
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        return PopulateNbtCompound(data);
    }

    private static NbtCompound PopulateNbtCompound(Dictionary<string, object>? data)
    {
        var compound = new NbtCompound();
        if (data != null)
            foreach (var entry in data)
            {
                // Handle each field explicitly
                if (entry.Key == "id" || entry.Key == "lvl")
                {
                    compound.Add(new NbtShort(entry.Key, Convert.ToInt16(entry.Value)));
                    continue;
                }

                switch (entry.Value)
                {
                    case int intValue:
                        compound.Add(new NbtInt(entry.Key, intValue));
                        break;
                    case long longValue:
                        if (longValue >= int.MinValue && longValue <= int.MaxValue)
                        {
                            compound.Add(new NbtInt(entry.Key, (int)longValue));
                        }
                        else
                        {
                            compound.Add(new NbtLong(entry.Key, longValue));
                        }

                        break;
                    case short shortValue:
                        compound.Add(new NbtShort(entry.Key, shortValue));
                        break;
                    case float floatValue:
                        compound.Add(new NbtFloat(entry.Key, floatValue));
                        break;
                    case double doubleValue:
                        compound.Add(new NbtDouble(entry.Key, doubleValue));
                        break;
                    case string stringValue:
                        compound.Add(new NbtString(entry.Key, stringValue));
                        break;
                    case Newtonsoft.Json.Linq.JArray jArrayValue:
                        var list = ConvertJArrayToNbtList(jArrayValue);
                        compound.Add(new NbtList(entry.Key, list));
                        break;
                    case Newtonsoft.Json.Linq.JObject jObjectValue:
                        var subCompound = PopulateNbtCompound(jObjectValue.ToObject<Dictionary<string, object>>());
                        compound.Add(new NbtCompound(entry.Key, CloneTags(subCompound.Tags)));
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported NBT type: {entry.Value.GetType()}");
                }
            }

        return compound;
    }

    private static List<NbtTag> ConvertJArrayToNbtList(Newtonsoft.Json.Linq.JArray jArray)
    {
        var nbtList = new List<NbtTag>();
        foreach (var item in jArray)
        {
            switch (item.Type)
            {
                case Newtonsoft.Json.Linq.JTokenType.Object:
                    var subCompound = PopulateNbtCompound(item.ToObject<Dictionary<string, object>>());
                    nbtList.Add(new NbtCompound(null, CloneTags(subCompound.Tags)));
                    break;
                case Newtonsoft.Json.Linq.JTokenType.Integer:
                    var value = (long)item;
                    if (value is >= short.MinValue and <= short.MaxValue)
                    {
                        nbtList.Add(new NbtShort(null, (short)value));
                    }
                    else if (value is >= int.MinValue and <= int.MaxValue)
                    {
                        nbtList.Add(new NbtInt(null, (int)value));
                    }
                    else
                    {
                        nbtList.Add(new NbtLong(null, value)); 
                    }
                    break;
                case Newtonsoft.Json.Linq.JTokenType.Array:
                    var subList = ConvertJArrayToNbtList((Newtonsoft.Json.Linq.JArray)item);
                    nbtList.Add(new NbtList(null, subList));
                    break;
                case Newtonsoft.Json.Linq.JTokenType.String:
                    nbtList.Add(new NbtString(null, ((string)item)!));
                    break;
                default:
                    throw new NotSupportedException($"Unsupported NBT type in list: {item.Type}");
            }
        }
        return nbtList;
    }

    private static List<NbtTag> CloneTags(IEnumerable<NbtTag> tags)
    {
        var clonedTags = new List<NbtTag>();
        foreach (var tag in tags)
        {
            clonedTags.Add(CloneTag(tag));
        }

        return clonedTags;
    }

    private static NbtTag CloneTag(NbtTag tag)
    {
        switch (tag)
        {
            case NbtByte nbtByte:
                return new NbtByte(nbtByte.Name, nbtByte.ByteValue);
            case NbtShort nbtShort:
                return new NbtShort(nbtShort.Name, nbtShort.ShortValue);
            case NbtInt nbtInt:
                return new NbtInt(nbtInt.Name, nbtInt.IntValue);
            case NbtLong nbtLong:
                return new NbtLong(nbtLong.Name, nbtLong.LongValue);
            case NbtFloat nbtFloat:
                return new NbtFloat(nbtFloat.Name, nbtFloat.FloatValue);
            case NbtDouble nbtDouble:
                return new NbtDouble(nbtDouble.Name, nbtDouble.DoubleValue);
            case NbtString nbtString:
                return new NbtString(nbtString.Name, nbtString.StringValue);
            case NbtByteArray nbtByteArray:
                return new NbtByteArray(nbtByteArray.Name, (byte[])nbtByteArray.ByteArrayValue.Clone());
            case NbtIntArray nbtIntArray:
                return new NbtIntArray(nbtIntArray.Name, (int[])nbtIntArray.IntArrayValue.Clone());
            case NbtLongArray nbtLongArray:
                return new NbtLongArray(nbtLongArray.Name, (long[])nbtLongArray.LongArrayValue.Clone());
            case NbtCompound nbtCompound:
                return new NbtCompound(nbtCompound.Name, CloneTags(nbtCompound.Tags));
            case NbtList nbtList:
                return new NbtList(nbtList.Name, CloneTags(nbtList));
            default:
                throw new NotSupportedException($"Unsupported NBT tag type: {tag.TagType}");
        }
    }
}