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
mae = 0
mse = 0
se = 0
length = len(predictions)

print("Predicted | Actual")
for i in range(100):
    print(predictions[i][0]," | ", testingLabelSet.iloc[i][0])
for i in range(length):
    mse += numpy.square(testingLabelSet.iloc[i][0] - predictions[i])[0]
    mae += numpy.abs(testingLabelSet.iloc[i][0] - predictions[i])[0]
    se += (numpy.abs(testingLabelSet.iloc[i][0] - numpy.sign(predictions[i]))/2)[0]
mae /= length
mse /= length
se /= length
print("mean ABS error: ",mae)
print("mean Squared error: ",mse)
print("mean sign error: ",se)
