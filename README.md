# VR Spatial Resolution Tester

The VR Spatial Resolution Tester is a VR line test tool. Similar to what is used for other imaging devices and displays, this program shows the user a set of line pairs and asks them to idenfiy the smallest possible line pair. In this case, the user changes the line sizes to the smallest possible point where the lines are still perceptably different from one another. This is repeated for lines in different orientations, shapes, and anchorings. The results, while subjective, can be analyzed to identify the visualization limits for a given user/headset combo, as well broadly for a spatial resolution estimate for a given headset.

This project was completed by Gillian Loparco with the [Medical Devies and Systems Lab](https://www.torontomu.ca/tavallaei/) under Dr. Ali Tavallaei.

# Specifications

- Unity 6.3 LTS Project (`6000.3.21f1`)
- Built with OpenXR, allowing for any headset to be used with minor control changes
    - Currently set to use the Meta Quest 3 and associated Quest Touch Plus controllers
- All line tests are sized and structured to mimic being 50cm away from the target
- Dynamic addition/removal of line shapes
- Local headset data logging

# How to Run
1. Clone this repo to your machine
```bash
git clone https://github.com/loparcog/VRSpatialResolitonTest.git
```
2. Add the project to and open the project in Unity Hub
    - To run as is, download the Unity Editor Version `6000.3.21f1` with Andriod Build Support. Otherwise, it should be ported to other Unity Editors with minimal issue.
3. Run the project through the...
    - **Unity Editor**
        - Ensure the `XR Device Simulator` is enabled
        - Enter play mode, utilizing controls displayed for the simulator
    - **VR Headset**
        - Disable the `XR Device Simulator`
        - Go to Build Profiles (File > Build Profiles)
        - Select Android from the list of platforms and click Switch Platform on the bottom right of the window
        - Within the Andriod settings, find the Run Device dropdown and select your headset
            - If the headset does not show up, ensure that the headset does not have any proprietary steps to connect to a computer
        - If your headset can **run projects from the Unity editor** (for example, utilizing Virtual Desktop for Meta Quest 3):
            - Enter play mode in Unity, porting visuals and controls to the connected headset
        - If you **cannot run project from Unity editor** of if you would like to **run the project as an application on the headset**:
            - In the Build Profiles window, select Build. Store this build anywhere on your machine, and Unity will port it to your headset
            - Access the local files on your headset and run the newly created .apk

# How to Modify
Any and all line tests are done in the [Assets](Assets) folder, which further explains the project structure and built-in modification points.

# TODO
- Assets README