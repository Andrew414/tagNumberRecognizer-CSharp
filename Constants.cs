using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagrec_S
{
    class Constants
    {
        #region Common constants

        public const int MAX_BRIGHTNESS = 255;

        #endregion

        #region Binary Matrix

        public const int BINARY_MATRIX_TRESHOLD = 1200;
        public const int BINARY_MATRIX_WIDTH = 120;
        public const int BINARY_MATRIX_HEIGHT = 200;

        #endregion

        #region Plate Finder

        public const int FINDER_BLUR_SIZE = 5;
        public const int FINDER_MAIN_THRESHOLD = 255;
        public const int FINDER_CANNY_FROM = 20;
        public const int FINDER_CANNY_TO = 50;
        public const int FINDER_MIN_WIDTH = 60;
        public const int FINDER_MIN_HEIGHT = 40;

        #endregion

        #region Sign Reader

        public const int SIGN_READER_BLUR_SIZE = 5;
        public const int SIGN_READER_MAIN_THRESHOLD = 255;
        public const int SIGN_READER_THRESHOLD_FROM = 0;
        public const int SIGN_READER_THRESHOLD_TO = 255;

        #endregion

        #region Plate Reader

        public const int PLATEREADER_NUMBERS_1ST_GROUP = 4;
        public const int PLATEREADER_LETTERS_2ND_GROUP = 2;
        public const int PLATEREADER_NUMBERS_3RD_GROUP = 1;

        public const int PLATEREADER_BLUR_SIZE = 5;
        public const int PLATEREADER_MAIN_THRESHOLD = 255;
        public const int PLATEREADER_THRESHOLD_FROM = 0;
        public const int PLATEREADER_THRESHOLD_TO = 255;

        public const int PLATEREADER_NUMBER_LEN = 7;

        public const int READER_LITTLE_ITEMS_ANGLE_FROM = 40;
        public const int READER_LITTLE_ITEMS_ANGLE_TO = 40;

        public const int READER_NO_ROTATE_HEIGHT_FROM = 90;
        public const int READER_NO_ROTATE_HEIGHT_TO = 110;
        public const int READER_NO_ROTATE_MAX_ANGLE = 5;

        public const int READER_90_ROTATE_HEIGHT_FROM = 90;
        public const int READER_90_ROTATE_HEIGHT_TO = 110;
        public const int READER_90_ROTATE_MAX_ANGLE = 100;
        public const int READER_90_ROTATE_MIN_ANGLE = 80;

        public const int READER_70_ROTATE_WIDTH_FROM = 80;
        public const int READER_70_ROTATE_WIDTH_TO = 100;
        public const int READER_70_ROTATE_MAX_ANGLE = 75;
        public const int READER_70_ROTATE_MIN_ANGLE = 65;
        public const int READER_70_DELTA_X = -10;
        public const int READER_70_DELTA_Y = -5;
        public const int READER_70_DELTA_WIDTH = 5;
        public const int READER_70_DELTA_HEIGHT = 10;

        public const int READER_DEFALUT_WIDTH = 650;
        public const int READER_DEFALUT_HEIGHT = 150;

        #endregion

    }
}
