namespace InfraSpy.Core
{
    using k8s;
    using k8s.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class K8SClient
    {
        private KubernetesClientConfiguration config;
        private IKubernetes client;

        /// <summary>
        /// Prevents a default instance of the <see cref="K8SClient"/> class from being created.
        /// </summary>
        private K8SClient()
        {
            config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            client = new Kubernetes(config);
        }

        /// <summary>
        /// Nested class for lazy instantiation of singleton
        /// </summary>
        private class NestedClient
        {
            /// <summary>
            /// Initializes the <see cref="NestedClient"/> class.
            /// </summary>
            static NestedClient()
            { }

            internal static readonly K8SClient k8sClient = new K8SClient();
        }

        public static K8SClient Instance { get => NestedClient.k8sClient; }

        public IList<V1Namespace> Namespaces { get => client.ListNamespace().Items; }

        public IList<V1Pod> GetPods(string ns)
        {
            return client.ListNamespacedPod(ns).Items;
        }

        public IList<V1Deployment> GetDeployments(string ns)
        {            
            return client.ListNamespacedDeployment(ns).Items;
        }

        public string GetPodLogSnapshot(string podName, string ns, int? tailLines = null)
        {
            var stream = client.ReadNamespacedPodLog(podName, ns, tailLines: tailLines);

            StreamReader reader = new StreamReader(stream);
            string logText = reader.ReadToEnd();

            return logText;
        }

        public string SearchInPodLog(string podName, string ns, string searchPhrase, int? tailLines = null)
        {
            var stream = client.ReadNamespacedPodLog(podName, ns, tailLines: tailLines);

            StreamReader reader = new StreamReader(stream);
            StringBuilder resultsBuilder = new StringBuilder();

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if(line.Contains(searchPhrase))
                {
                    resultsBuilder.AppendLine(line);
                }
            }

            return resultsBuilder.ToString();
        }
    }
}
