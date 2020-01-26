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

                case RecognizedObjects.Strawberry:
                    return "Strawberries";
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

                case RecognizedObjects.Strawberry:
                    return "images/straw.jpg";
            }

            return null;
        }

    }
}