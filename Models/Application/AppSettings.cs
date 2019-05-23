namespace Models.Application {
	public class AppSettings {
		public ConnectionStrings ConnectionStrings { get; set; }

		public FeatureToggles FeatureToggles { get; set; }

        public RedisSettings RedisSettings { get; set; }
    }
}