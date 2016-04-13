using UnityEngine;

static class InputHandler {
    private static string[/*,*/] inputs = { /*{*/"1u", "1d", "1l", "1r", "1A", "1B", "1Lt", "1Rt"/*}*/,
                                            /*{*/ "2u", "2d", "2l", "2r", "2A", "2B", "2Lt", "2Rt" } /*}*/;
    // return the instructed movement direction for the given player
    public static int whichMove(int player) {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButton(inputs[player*8 + i]))
            {
                return i + 1;
            }
        }
        return 0;
    }

    public static bool buttonUp(int player, button btn) {
        return (Input.GetButtonDown(inputs[player*8+(int)btn]));
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
