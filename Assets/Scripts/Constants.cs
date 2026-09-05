public class Constants
{
    /*
        CONTROLS ENUM
        Globally referenced controller values, change the Scene Controller
        script arguments on the Script Manager object to adjust their
        references.

        UP: Makes lines larger, increment data in User Data Screen
        DOWN: Makes lines smaller, decrement data in User Data Screen
        BUNTTON: Confirmation button, used to go to the next scene
        SECONDBUTTON: Reverse button, used to go back to last scene
        TRIGGER: Toggle fine-tuning, change data selection in User Data Screen
    */
    public enum CONTROLS
    {
        UP,
        DOWN,
        BUTTON,
        SECONDBUTTON,
        TRIGGER
    }

    /*
        LINE TYPE ENUM
        Which line shapes shoudl be tested. Each listed line type will
        run all listed LINE ORIENTATION tests before continuing to the
        next test. All will be repeated for static and dynamic tests.

        All options can be found in Assets/Resources/Lines 
    */
    public enum LINE_TYPE
    {
        /*
            Add and remove any items here to change test
        */
        Infinite,
        E
    }

    /*
        LINE ORIENTATION ENUM
        Set of tests performed for each selected line type. Add or remove
        any from this enum to change what tests are performed

        HORIZONTAL: Lines laid left to right
        VERTICAL: Lines laid top to bottom
        DIAGONAL: Lines laid bottom left to top right
    */
    public enum LINE_ORIENTATION
    {
        HORIZONTAL,
        VERTICAL,
        DIAGONAL
    }

    // LOG DATA LOCATION
    public const string LOGFILE = "VRRTData.csv";
    



}
