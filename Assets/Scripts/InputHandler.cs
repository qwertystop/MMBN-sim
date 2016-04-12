using UnityEngine;

static class InputHandler {
    private static string[,] inputs = { {"1u", "1d", "1l", "1r", "1A", "1B", "1Lt", "1Rt"},
                                            { "2u", "2d", "2l", "2r", "2A", "2B", "2Lt", "2Rt" } };
    // return the instructed movement direction for the given player
    public static int whichMove(int player) {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButton(inputs[player, i]))
            {
                return i + 1;
            }
        }
        return 0;
    }

    public static bool buttonUp(int buttonIndex, int player) {
        return (Input.GetButtonUp(inputs[player, buttonIndex]));
    }
}
