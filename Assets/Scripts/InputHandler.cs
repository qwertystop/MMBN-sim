using UnityEngine;

static class InputHandler {
    private static string[,] keys = { {"w", "s", "a", "d", "b", "n", "g", "h"},
                                      {"up", "down", "left", "right", "[", "]", "-", "="} };
    // return the instructed movement direction for the given player
    public static int whichMove(int player) {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(keys[player, i]))
            {
                return i + 1;
            }
        }
        return 0;
    }

    public static bool buttonUp(int player, button btn) {
        return (Input.GetKeyUp(keys[player,(int)btn]));
    }

    public static bool buttonDown(int player, button btn) {
        return (Input.GetKeyDown(keys[player, (int)btn]));
    }

    public static bool buttonHeld(int player, button btn) {
        return (Input.GetKey(keys[player, (int)btn]));
    }

    public enum button {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3,
        A = 4,
        B = 5,
        L = 6,
        R = 7
    }
}
