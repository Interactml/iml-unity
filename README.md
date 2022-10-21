# InteractML, an Interactive Machine Learning Visual Scripting framework for Unity3D

<p align="center">
  <img src="https://github.com/Interactml/site/blob/master/images/title_web.png">
</p>

InteractML is an Unity3d Plugin that enables developers to configure, train, and use Interactive Machine Learning (IML) systems within the game editor. Using visual scripting developers, designers and artists can visualise incoming data, configure game inputs (e.g., specifying what data to extract from sensors or objects in the game); train and refine ML models (by iteratively adding new training examples in realtime); and connect the ML model outputs (the real-time predictions calculated based on the training data) to other objects/scripts in the game scene. In addition, since InteractML doesn't rely on external software, the ML models can be trained and/or refined by player-provided examples in the final version of the game.

### Key features
* Lightweight machine learning models: Classification, Regression and Dynamic Time Warping
* Node visual scripting interface
* You can code your own nodes to satisfy needs not currenly covered
* Integration with any script to pipe data in/out
* Supported from Unity 5.3 and up
* Windows/Mac full support 

### Limitations
* Beta stage
* Anything not Windows/Mac not tested or currently supported

### Installation

## Install InteractML as a released package
The releases page is currently *heavily* outdated, don't download from there! While we are working to produce a new stable package, you can clone this branch or install InteractML to your project as a git submodule (more info below).

~~Don't download the master branch! It currently contains several dependencies that might break your project. Instead, download one of the relase packages as follows:~~

~~1) Download the latest UnityPackage from the InteractML release github page here: [https://github.com/Interactml/iml-unity/releases](https://github.com/Interactml/iml-unity/releases)~~

~~2) With your unity project open, navigate to the top menu bar and select _Assets_. In the Assets menu select _Import Package_, then select _Custom Package_.~~

~~3) Find the downloaded unity package in your file system and click _Open_. Wait for unity to prepare the package.~~

~~4) In the Import Unity Package window that pops up click _Import_. Wait for unity to import the package.~~

That’s it! The InteractML folder will appear in your assets folder along with a folder of examples to start you off.

## Install InteractML as a git submodule
```
# initiate git in folder (if not done already)
git clone URL [your_repository]

# go to your repo's folder
cd [your_repository]

# add InteractML as a submodule
git submodule add -b VRInterface --force https://github.com/Interactml/iml-unity.git Assets/iml-unity

# If you don't want the clutter from the examples...
# go to iml submodule folder
cd Assets/iml-unity/
# set the iml submodule to only use the Assets folder 
git sparse-checkout init --cone
git sparse-checkout set Assets
# leave submodule folder
cd ../..
```

### Documentation
Visit the [wiki to have a look at our how-to guides.](https://github.com/Interactml/iml-unity/wiki)

### Machine Learning Algorithms
The machine learning back-end is the [RapidLib](https://github.com/mzed/RapidLib) C++ library. RapidLib's implementation mirrors that of [Wekinator](http://www.wekinator.org/), whose current choice of algorithms and default parameterisations have been informed by more than a decade of use by creative practitioners, ensuring that they work well off-the-shelf for many IML tasks involving small training sets and human motion modeling tasks. InteractML currently implements:
* Classification through a [kNN algorithm](https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm).
* Regression through a [Multilayer Perceptron (MLP)](https://en.wikipedia.org/wiki/Multilayer_perceptron) with one hidden layer. 
* [Dynamic Time Warping (DTW)](https://en.wikipedia.org/wiki/Dynamic_time_warping) as a classification algorithm for a time series dataset (i.e. gesturing over time).

### Dependencies
InteractML is built with 
* [xNode](https://github.com/Siccity/xNode) - xNode is visual node framework that lets you view and edit node graphs inside Unity.
* [JsonNetForUnity](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347) - brings the power of Json and Bson serialization to Unity with support for 4.7.2 and up and is compatible with both .NET and IL2CPP backends.
* [RapidLib](https://github.com/mzed/RapidLib) - RapidLib is a lightweight library for interactive machine learning written in C++. It currently features classification (using kNN), regression (multilayer perceptron), and series classification (using dynamic time warping).


