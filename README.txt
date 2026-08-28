# The VR Spatial Resolution Tester

This project was completed by Gillian Loparco with the [Medical Devies and Systems Lab](https://www.torontomu.ca/tavallaei/) under Dr. Ali Tavallaei.

# Why?

While investigating the possibility of consumer-available VR headsets for telesurgery processes, there seemed to be a gap on identifying a practical measure of resolution for a user. Of course, many headsets have their resolutions and pixel-per-degree values listed, but due to the complex rendering processes, custom lenses, and dynamics of head movement, these values cannot be directly translated to find the smallest object perceivable for a given headset.

This project was created as a possible answer to identifying the spatial resolution of a given headset for a given user, as well as to identify the spatial resolution of the Meta Quest 3 through mass trial analysis.

# What is it?

The VR Spatial Resolution Tester, at its core, is a 3D line test. Similar to what is used for other imaging devices and displays, the program shows the user a set of line pairs and asks them to idenfiy the smallest possible line pair. In this case, the user changes the line sizes to the smallest possible point where the lines are still perceptably different from one another. This is repeated for lines in a horizontal, vertical, and diagonal orientation, as well as for lines anchored to the headset (essentially acting as a static image on the headset) and anchored in space, oscillating slightly and allowing for head movement adjustments to simulate a more realistic environment.

These tests are fluffed for data collection, surrounded by an introductory tutorial, user data collection screen, and end screen.

# How do I run it?

At the time of writing, this project is running on Unity 6000.3.21f1, however it should port easily to newer versions as long as there aren't major structural changes. Clone this repo to your local machine and open it within Unity. To run it on your own machine, enable the XR Simulation tool. Otherwise, disable this and connect a VR headset to use as a source. This was also built to be deployed directly on a headset, so feel free to build and directly upload as an application.

# How can I change it?

For any lower level information on the project, please see the `Assets` folder and its associated README's.

# Issues

Feel free to use the Issues board on GitHub for any fixes, updates, or questions.