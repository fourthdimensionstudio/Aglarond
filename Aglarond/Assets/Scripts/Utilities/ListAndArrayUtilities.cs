using System.Collections.Generic;

public static class ListAndArrayUtilities {
    public static T RandomOrDefault<T>(this IList<T> _list) {
        if (_list.Count == 0) {
            return default(T);
        }

        return _list[UnityEngine.Random.Range(0, _list.Count)];
    }

    public static void Shuffle<T>(this IList<T> _list) {
        int tempCounter = _list.Count;
        while (tempCounter > 1) {
            tempCounter--;
            int indexToShuffle = UnityEngine.Random.Range(0, _list.Count);
            T value = _list[indexToShuffle];
            _list[indexToShuffle] = _list[tempCounter];
            _list[tempCounter] = value;
        }
    }
}