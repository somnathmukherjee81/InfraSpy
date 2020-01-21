namespace InfraSpy
{
    using System;
    using System.Globalization;
    using InfraSpy.Core;

    public class Program
    {
        public static void Main(string[] args)
        {
            string ns = "smartpoint-chandra";
            Console.WriteLine($"Pods in {ns} namespace");
            Console.WriteLine("==============================");
            foreach (var pod in K8SClient.Instance.GetPods(ns))
            {
                Console.WriteLine(pod.Metadata.Name);
            }

            Console.Write("Enter pod name to get logs: ");
            string podName = Console.ReadLine();

            Console.Write("Enter number of lines to display: ");
            string tail = Console.ReadLine();
            int tailLines = Convert.ToInt32(tail, NumberFormatInfo.InvariantInfo);

            string logText = K8SClient.Instance.GetPodLogSnapshot(podName, ns, tailLines);


            Console.WriteLine($"{tailLines} lines of logs from {podName}");
            Console.WriteLine("==============================");
            Console.WriteLine(logText);

            Console.Write("Search Phrase: ");
            string searchPhrase = Console.ReadLine();
            string searchResult = K8SClient.Instance.SearchInPodLog(podName, ns, searchPhrase, tailLines);

            Console.WriteLine($"Searching the phrase '{searchPhrase}' in {tailLines} lines of logs from {podName}");
            Console.WriteLine("==============================");
            Console.WriteLine(searchResult);
        }
    }
}
