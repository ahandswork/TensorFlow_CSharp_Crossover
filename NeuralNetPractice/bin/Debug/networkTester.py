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
mape = 0
mae = 0
mse = 0
for i in range(len(predictions)):
    mse += numpy.square(testingLabelSet.iloc[i][0] - predictions[i])
    mae += numpy.abs(testingLabelSet.iloc[i][0] - predictions[i])
    mape += numpy.abs((1 - predictions[i]/testingLabelSet.iloc[i][0])[0])
mape /= len(predictions)
numpy.std()
print("mean ABS error: ",mae * 100)
print("mean Squared error: ",mse * 100)
print("mean ABS percentage error: ",mape * 100)
