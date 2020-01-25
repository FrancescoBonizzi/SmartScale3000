using SmartWeightDevice.Domain;

namespace SmartWeightDevice.Extensions
{
    public static class Mappings
    {
        public static string DisplayText(this RecognizedObjects recognizedObject)
        {
            switch (recognizedObject)
            {
                case RecognizedObjects.Apple:
                    return "Apples";

                case RecognizedObjects.Orange:
                    return "Oranges";

                case RecognizedObjects.Banana:
                    return "Bananas";
            }

            return "WAT?!";
        }

        public static string MainImagePath(this RecognizedObjects recognizedObject)
        {
            switch (recognizedObject)
            {
                case RecognizedObjects.Apple:
                    return "images/yellow-apple.jpg";

                case RecognizedObjects.Orange:
                    return "images/oranges.jpg";

                case RecognizedObjects.Banana:
                    return "images/bananas.jpg";
            }

            return null;
        }

    }
}