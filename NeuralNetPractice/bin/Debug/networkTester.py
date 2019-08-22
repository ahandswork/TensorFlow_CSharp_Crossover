from __future__ import absolute_import, division, print_function, unicode_literals

import pathlib
import os

#import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns

import tensorflow as tf
import numpy
from tensorflow import keras
from tensorflow.keras import layers

MODEL_SAVE_LOCATION = "Models\\main_model"

testingSet = pd.read_csv("PostProcessData/testingSet.csv")
testingLabelSet = pd.read_csv("PostProcessData/testingLabelSet.csv")

currentModel = keras.models.load_model(MODEL_SAVE_LOCATION)

predictions = currentModel.predict(testingSet)
mpe = 0
for i in range(len(predictions)):
    mpe += (1 - predictions[i]/testingLabelSet.iloc[i][0])[0]
print("mean percentage error: ",mpe * 100)
