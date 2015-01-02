using System.Collections.Generic;

namespace StreamLauncher.Dtos
{
    public class GetLiveStreamResponseDto
    {
        public int Id { get; set; }
        public List<StreamDto> HDstreams { get; set; }
        public List<StreamDto> SDstreams { get; set; }
        public List<StreamDto> TrueLiveSD { get; set; }
        public List<StreamDto> TrueLiveHD { get; set; }
    }

    public class StreamDto
    {
        public string Type { get; set; }
        public string Src { get; set; }
        public string Location { get; set; }
    }
}