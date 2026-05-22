using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    public static void Main()
    {
        Client clientelle = nouveauClient();
        GenererJson(clientelle);

    }

    static Dictionary<string, int> gestion_Vehicule(Dictionary<string, int> voiture)
    {
        // on crée un nouveau dictionnaire pour stocker les résultats propres
        Dictionary<string, int> dicoFinal = new Dictionary<string, int>();
        foreach(string cleBrute in voiture.Keys)
        {
            // on récupère le nom du code sans le dernier caractère (qui est l'indice)
            // Substring(0, longueur - 1) prend tout sauf le dernier caractère
            string codeSeul = cleBrute.Substring(0, cleBrute.Length - 1);
            // on récupère la quantité qui était associée à cette clé
            int quantite = voiture[cleBrute];
            // on ajoute au nouveau dictionnaire : "code" -> quantité
            dicoFinal.Add(codeSeul, quantite);
        }
        return dicoFinal;
    }

    static Dictionary<string, int> ajoutVehicule(string date, string heure, int liaison)
    {
        float[] groix_vehicule = new float[] {4.70f, 8.20f, 11.00f, 16.45f, 23.10f, 66.05f, 96.05f, 114.80f, 174.45f, 210.90f, 330.20f};
        float[] belleIle_vehicule = new float[] {4.70f, 8.20f, 11.00f, 16.45f, 23.35f, 66.40f, 98.50f, 117.20f, 176.90f, 213.35f, 332.70f};
        string[] categorie_vehicule = new string[] {"1 Trottinette électrique", "2 : Vélo ou remorque à vélo", "3 : Vélo électrique", "4 : Vélo cargo ou tandem", "5 : Deux-roues <= 125 cm3", "6 : Deux-roues > 125cm3", "7 : Voiture moins de 4 m", "8 : Voiture de 4 m à 4.39 m", "9 : Voiture de 4.40 m à 4.79 m", "10 : Voiture 4.80 m et plus", "11 : Camping-car - véhicule plus de 2.10 de haut"};
        string[] code_vehicule = new string[] {"trot", "velo", "velelec", "cartand", "mobil", "moto", "cat1", "cat2", "cat3", "cat4", "camp"};
        // c'est ce code qui sera placé dans le json
        Console.WriteLine("Combien d'éléments ?");
        int nbElements = int.Parse(Console.ReadLine());
        // variable qui permettra de,boucler jusqu'au nombre de véhicules voulu
        Dictionary<string, int> dico = new Dictionary<string, int>();
        if(nbElements > 0 && liaison != 4 && date != "6" && heure != "16:15")
        // il y a une heure à la liaison 4 dans laquelle les véhicules sont proscrits
        {
            switch(liaison)
            {
                case 1 :
                    dico = menu_voiture(categorie_vehicule, code_vehicule, groix_vehicule, nbElements);
                    break;
                case 2 :
                    dico = menu_voiture(categorie_vehicule, code_vehicule, groix_vehicule, nbElements);
                    break;
                case 3 :
                    dico = menu_voiture(categorie_vehicule, code_vehicule, belleIle_vehicule, nbElements);
                    break;
                case 4 :
                    dico = menu_voiture(categorie_vehicule, code_vehicule, belleIle_vehicule, nbElements);
                    break;
            }
        }
        return dico;
    }

    static Dictionary<string, int> ajoutPassager(string date, string heure, int liaison)
    {
        float[] groix_passager = new float[] {18.75f, 13.80f, 11.25f, 0.00f, 3.35f};
        float[] belleIle_passager = new float[] {18.80f, 14.10f, 11.65f, 0.00f, 3.35f}; 
        string[] categorie_passager = new string[] {"1 Adulte 26 ans et plus", "2 : Jeune 18 à 25 ans inclus", "4 : Enfant 4 à 17 ans inclus", "5 : Bébé moins de 4 ans", "6 : Animal de compagnie"};
        string[] code_passager = new string[] {"adu26p", "jeu1825", "enf417", "bebe", "ancomp"};
        // c'est ce code qui sera placé dans le json
        Console.WriteLine("Combien de passagers ?");
        int nbPassagers = int.Parse(Console.ReadLine());
        Dictionary<string, int> dico = new Dictionary<string, int>();
        if(nbPassagers > 0)
        // si l'on a des passagers
        {
            switch(liaison)
            // selon la liaison, les prix sont différents
            {
                case 1 :
                    dico = menu_passager(categorie_passager, code_passager, groix_passager, nbPassagers);
                    break;
                case 2 :
                    dico = menu_passager(categorie_passager, code_passager, groix_passager, nbPassagers);
                    break;
                case 3 :
                    dico = menu_passager(categorie_passager, code_passager, belleIle_passager, nbPassagers);
                    break;
                case 4 :
                    dico = menu_passager(categorie_passager, code_passager, belleIle_passager, nbPassagers);
                    break;
            }
        }
        return dico;
    }

    static float prixTotal(int liaison, Dictionary<string, int> passager, Dictionary<string, int> vehicule)
    {
        float total = 0.00f; 
        float[] groix_passager = new float[] {18.75f, 13.80f, 11.25f, 0.00f, 3.35f};
        float[] belleIle_passager = new float[] {18.80f, 14.10f, 11.65f, 0.00f, 3.35f};
        float[] groix_vehicule = new float[] {4.70f, 8.20f, 11.00f, 16.45f, 23.10f, 66.05f, 96.05f, 114.80f, 174.45f, 210.90f, 330.20f};
        float[] belleIle_vehicule = new float[] {4.70f, 8.20f, 11.00f, 16.45f, 23.35f, 66.40f, 98.50f, 117.20f, 176.90f, 213.35f, 332.70f};
        float[] tarifsPassagers;
        float[] tarifsVehicules;
        // Choix de la grille tarifaire (Lorient/Groix ou Quiberon/Belle-Ile)
        if (liaison == 1 || liaison == 2)
        {
            tarifsPassagers = groix_passager;
            tarifsVehicules = groix_vehicule;
        }
        else
        {
            tarifsPassagers = belleIle_passager;
            tarifsVehicules = belleIle_vehicule;
        }
        foreach (string cleP in passager.Keys)
        {
            // La valeur dans le dictionnaire est l'indice de sa catégorie
            int indexCategorie = passager[cleP];
            // on ajoute le prix à cet indice au total
            total += tarifsPassagers[indexCategorie];
        }
        foreach (string cleV in vehicule.Keys)
        {
            string indexExtrait = espace(cleV); 
            // On récupère l'indice possible à deux chiffres
            int indexPrix = int.Parse(indexExtrait); 
            // On convertit en nombre
            int quantite = vehicule[cleV];
            total += tarifsVehicules[indexPrix] * quantite;
        }
        return total;
    }

    static string espace(string chaine)
    // l'indice pour accéder à chaque type de véhicule sont stockés à a fin de chaque clé véhicule
    // ces deux éléments distincts sont séparés par un espace
    // le but de cette fonction est d'extraire uniquement l'indice 
    {
        string indice = "";
        int lettre = -1; 
        // Initialisé à -1 pour vérifier si on a trouvé l'espace
        bool flag = true;
        int debordement = 0;
        while(flag && debordement < chaine.Length)
        {
            if(chaine[debordement] == ' ') 
            {
                lettre = debordement;
                flag = false;
            }
            debordement++;
        }

        if (lettre != -1)
        {
            // On part de lettre + 1
            // La longueur est : (Longueur totale) - (Position de l'espace + 1)
            indice = chaine.Substring(lettre + 1, chaine.Length - (lettre + 1));
        }
        return indice;
    }

    static Dictionary<string, int> menu_voiture(string[] categorie, string[] code_categorie, float[] prix, int nbElements)
    // gère le stockage des voitures
    {
        int numObjet;
        // cette variable stockera l'indice de l'élément choisi par le client 
        Dictionary<string, int> dico = new Dictionary<string, int>();
        // le dictionnaire sera composé du nom de l'élément comme clé suivi de l'indice de ce même élément dans le tableau categorie
        for(int i = 0; i < prix.Length; i++)
        {
            Console.WriteLine($"{categorie[i]} pour {prix[i]} €");
            // affichage du prix de chaque élément
        }
        for(int j = 0; j < nbElements; j++)
        {
            Console.WriteLine("Saisissez le numéro");
            numObjet = int.Parse(Console.ReadLine());
            if(numObjet >= 0 && numObjet < prix.Length)
            // on vérifie si la valeur de l'indice est bien conforme
            {
                string maCle = code_categorie[numObjet] + " " + numObjet; // On prépare la clé une fois
                if(dico.ContainsKey(maCle))
                {
                    dico[maCle] += 1; 
                }
                else 
                {
                    dico.Add(maCle, 1);
                }
                
            }
        }
        return dico;
    }

    static Dictionary<string, int> menu_passager(string[] categorie, string[] code_categorie, float[] prix, int nbElements)
    // gère les passagers
    {
        int cat;
        // indice de la catégorie du passager
        string nom;
        string prenom;
        Dictionary<string, int> dico = new Dictionary<string, int>();
        for(int i = 0; i < categorie.Length; i++)
        {
            Console.WriteLine($"{i} : {categorie[i]} : {prix[i]}");
            // on affiche les valeurs correspondant à chaque catégorie de passager
            // à la fin, on a le prix pour chaque catégorie de passager
        }
        for(int j = 0; j < nbElements; j++)
        {
            Console.WriteLine("Saisissez un nom");
            nom = Console.ReadLine();
            Console.WriteLine("Saisissez un prenom");
            prenom = Console.ReadLine();
            Console.WriteLine("Saisissez le numero de categorie");
            cat = int.Parse(Console.ReadLine());
            dico.Add(nom + ";" + prenom + ";" + code_categorie[cat], cat);
            // le dictionnaire contient nom, prénom, catégorie en tant que clé et son indice de catégorie en tant que valeur
        }
        return dico;
    }


    static Client nouveauClient()
    {
        Dictionary <string, int> passager = new Dictionary<string, int>();
        Dictionary <string, int> vehicule_temp = new Dictionary<string, int>();
        string[] dateJuste = new string[2];
        Console.WriteLine("1 : Lorient -> Groix, 2 : Groix -> Lorient, 3 : Quiberon -> Le Palais, 4 : Le Palais -> Quiberon");
        int liaison = int.Parse(Console.ReadLine());
        Console.WriteLine("Nom ?");
        string nom = Console.ReadLine();
        // sous quel nom est réservé le trajet
        Console.WriteLine("liaison ?");
        dateJuste = menu_traversee(liaison);
        // obtenir une date conforme selon la traversée 
        string date = "2025-11-" + dateJuste[0];
        string heure = dateJuste[1];
        // dateJuste est un tableau contenant le jour ainsi que l'heure
        passager = ajoutPassager(date, heure, liaison);
        vehicule_temp = ajoutVehicule(date, heure, liaison);
        float prix = prixTotal(liaison, passager, vehicule_temp);
        Dictionary <string, int> vehicule = new Dictionary<string, int>();
        vehicule = gestion_Vehicule(vehicule_temp);
        // le dictionnaire de base qui contenait les véhicules avait été arrangé pour y contenir les prix
        // la version finale a besoin de contenir le nombre de véhicule de même type 
        Client client = new Client(nom, date, heure, liaison, passager, vehicule, prix);
        // une fois toutes les informations acquises, on peut générer le json
        return client;

    }

    static string[] menu_traversee(int liaison)
    // le but de la fonction est d'obtenir une date conforme
    {
        string[] tab = new string[2];
        switch(liaison)
        {
            case 1 :
                tab = lorient_Groix();
                break;
            case 2 :
                tab = groix_Lorient();
                break;
            case 3 :
                tab = quiberon_lePalais();
                break;
            case 4 :
                tab = lePalais_Quiberon();
                break;
        }
        return tab;
    }

    static string[] lorient_Groix()
    {
        string[] tab = new string[2];
        string ligne;
        char[] separateur;
        string[] temp;
        List<string> liste = new List<string>();
        
        int compteur = 1;
        if(File.Exists("lorient_Groix.txt"))
        {
            FileStream fs = new FileStream("lorient_Groix.txt", FileMode.Open, FileAccess.Read);
            StreamReader lorient_Groix = new StreamReader(fs);
            while(!lorient_Groix.EndOfStream)
            {
                ligne = lorient_Groix.ReadLine();
                separateur = new char[] {';'};
                temp = ligne.Split(separateur);
                for(int j = 0; j < temp.Length; j++)
                {
                    Console.Write(" " + temp[j]);
                }
                Console.WriteLine(" ");
            }
            
            lorient_Groix.Close();
        }
        Console.WriteLine("Quel jour entre 1 et 30 ?");
        int jour = int.Parse(Console.ReadLine());
        if(File.Exists("lorient_Groix.txt"))
        {
            FileStream fs = new FileStream("lorient_Groix.txt", FileMode.Open, FileAccess.Read);
            StreamReader lorient_Groix = new StreamReader(fs);
            while(!lorient_Groix.EndOfStream)
            { 
                ligne = lorient_Groix.ReadLine();
                if(compteur == jour)
                {
                    
                    separateur = new char[] {';'};
                    temp = ligne.Split(separateur);
                    for(int i = 1; i < temp.Length; i++)
                    {
                        liste.Add(temp[i].ToString());
                    }
                }
                compteur += 1;
            }
            
        }
        tab = verifDate(liste, jour);
        return tab;
    }

    static string[] groix_Lorient()
    {
        string[] tab = new string[2];
        string ligne;
        char[] separateur;
        string[] temp;
        List<string> liste = new List<string>();
        
        int compteur = 1;
        if(File.Exists("groix_Lorient.txt"))
        {
            FileStream fs = new FileStream("groix_Lorient.txt", FileMode.Open, FileAccess.Read);
            StreamReader groix_Lorient = new StreamReader(fs);
            while(!groix_Lorient.EndOfStream)
            {
                ligne = groix_Lorient.ReadLine();
                separateur = new char[] {';'};
                temp = ligne.Split(separateur);
                for(int j = 0; j < temp.Length; j++)
                {
                    Console.Write(" " + temp[j]);
                }
                Console.WriteLine(" ");
            }
            groix_Lorient.Close();
        }
        
        Console.WriteLine("Quel jour entre 1 et 30 ?");
        int jour = int.Parse(Console.ReadLine());
        if(File.Exists("groix_Lorient.txt"))
        {
            FileStream fs = new FileStream("groix_Lorient.txt", FileMode.Open, FileAccess.Read);
            StreamReader groix_Lorient = new StreamReader(fs);
            while(!groix_Lorient.EndOfStream)
            { 
                ligne = groix_Lorient.ReadLine();
                if(compteur == jour)
                {
                    separateur = new char[] {';'};
                    temp = ligne.Split(separateur);
                    for(int i = 1; i < temp.Length; i++)
                    {
                        liste.Add(temp[i].ToString());
                    }
                }
                compteur += 1;
            }
            
        }
        tab = verifDate(liste, jour);
        return tab;
    }

    static string[] lePalais_Quiberon()
    {
        string[] tab = new string[2];
        string ligne;
        char[] separateur;
        string[] temp;
        List<string> liste = new List<string>();
        
        int compteur = 1;
        if(File.Exists("lePalais_Quiberon.txt"))
        {
            FileStream fs = new FileStream("lePalais_Quiberon.txt", FileMode.Open, FileAccess.Read);
            StreamReader lePalais_Quiberon = new StreamReader(fs);
            while(!lePalais_Quiberon.EndOfStream)
            {
                ligne = lePalais_Quiberon.ReadLine();
                separateur = new char[] {';'};
                temp = ligne.Split(separateur);
                for(int j = 0; j < temp.Length; j++)
                {
                    Console.Write(" " + temp[j]);
                }
                Console.WriteLine(" ");
            }
            lePalais_Quiberon.Close();
        }
        
        Console.WriteLine("Quel jour entre 1 et 30 ?");
        int jour = int.Parse(Console.ReadLine());
        if(File.Exists("lePalais_Quiberon.txt"))
        {
            FileStream fs = new FileStream("lePalais_Quiberon.txt", FileMode.Open, FileAccess.Read);
            StreamReader lePalais_Quiberon = new StreamReader(fs);
            while(!lePalais_Quiberon.EndOfStream)
            { 
                ligne = lePalais_Quiberon.ReadLine();
                if(compteur == jour)
                {
                    
                    separateur = new char[] {';'};
                    temp = ligne.Split(separateur);
                    for(int i = 1; i < temp.Length; i++)
                    {
                        liste.Add(temp[i].ToString());
                    }
                }
                compteur += 1;
            }
            
        }
        tab = verifDate(liste, jour);
        return tab;
    }

    static string[] quiberon_lePalais()
    {
        string[] tab = new string[2];
        string ligne;
        char[] separateur;
        string[] temp;
        List<string> liste = new List<string>();
        int compteur = 1;
        if(File.Exists("quiberon_lePalais.txt"))
        {
            FileStream fs = new FileStream("quiberon_lePalais.txt", FileMode.Open, FileAccess.Read);
            StreamReader quiberon_lePalais = new StreamReader(fs);
            while(!quiberon_lePalais.EndOfStream)
            {
                ligne = quiberon_lePalais.ReadLine();
                separateur = new char[] {';'};
                temp = ligne.Split(separateur);
                for(int j = 0; j < temp.Length; j++)
                {
                    Console.Write(" " + temp[j]);
                }
                Console.WriteLine(" ");
            }
            quiberon_lePalais.Close();
        }
        
        Console.WriteLine("Quel jour entre 1 et 30 ?");
        int jour = int.Parse(Console.ReadLine());
        if(File.Exists("quiberon_lePalais.txt"))
        {
            FileStream fs = new FileStream("quiberon_lePalais.txt", FileMode.Open, FileAccess.Read);
            StreamReader quiberon_lePalais = new StreamReader(fs);
            while(!quiberon_lePalais.EndOfStream)
            {
                ligne = quiberon_lePalais.ReadLine();
                if(compteur == jour)
                {
                    separateur = new char[] {';'};
                    temp = ligne.Split(separateur);
                    for(int i = 1; i < temp.Length; i++)
                    {
                        liste.Add(temp[i].ToString());
                
                    }
                    
                }
                compteur += 1;
            }
            
        }
        tab = verifDate(liste, jour);
        return tab;
    }

    static string[] verifDate(List<string> liste, int jour)
    // le but de la fonction est de sélectionner une date et une heure conforme
    {
        string[] tab = new string[2];
        bool flag = true;
        while(flag)
        // tant qu'on n'a pas une date conforme
        {
            Console.WriteLine("date ?");
            string date = Console.ReadLine();
            if(liste.Contains(date))
            // si la date est conforme
            {
                tab[0] = jour.ToString();
                tab[1] = date;
                flag = false;
                // la deuxième condition étant validée, on sort
            }
        }
        return tab;
    }



    struct Client
    {
        public string nom;
        public string date;
        public string depart;
        public int liaison;
        public Dictionary<string, int> passager;
        public Dictionary<string, int> vehicule;
        public float prix;

        public Client(string nm, string dt, string dp, int ls, Dictionary<string, int> pass, Dictionary<string, int> vh, float px)
        {
            nom = nm;
            date = dt;
            depart = dp;
            liaison = ls;
            passager = pass;
            vehicule = vh;
            prix = px;
        }
    }

    static void GenererJson(Client c)
    {
        // on récupère l'heure actuelle pour l'horodatage
        string horodatage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // début du fichier json
        string json = "[\n";
        json += "  {\n";

        // section réservation
        json += "    \"reservation\": {\n";
        json += $"      \"nom\": \"{c.nom}\",\n";
        json += $"      \"idLiaison\": {c.liaison},\n";
        json += $"      \"date\": \"{c.date}\",\n";
        json += $"      \"heure\": \"{c.depart}\",\n";
        json += $"      \"horodatage\": \"{horodatage}\"\n";
        json += "    },\n";

        // section passager
        json += "    \"passagers\": [\n";
        int i = 0;
        foreach (var p in c.passager)
        {
            // On sépare la clé "nom;prenom;code" pour extraire les infos
            string[] infos = p.Key.Split(';');
            json += "      {\n";
            json += $"        \"nom\": \"{infos[0]}\",\n";
            json += $"        \"prenom\": \"{infos[1]}\",\n";
            json += $"        \"codeCategorie\": \"{infos[2]}\"\n";
            json += "      }";
            // On ajoute une virgule sauf pour le dernier passager
            if (i < c.passager.Count - 1) json += ",";
            json += "\n";
            i++;
        }
        json += "    ],\n";

        // section véhicule
        json += "    \"vehicules\": [\n";
        int j = 0;
        foreach (var v in c.vehicule)
        {
            json += "      {\n";
            json += $"        \"codeCategorie\": \"{v.Key}\",\n";
            json += $"        \"quantite\": {v.Value}\n";
            json += "      }";
            // On ajoute une virgule sauf pour le dernier véhicule
            if (j < c.vehicule.Count - 1) json += ",";
            json += "\n";
            j++;
        }
        json += "    ]\n";

        // Fin du json
        json += "  }\n";
        json += "]";

        // Écriture physique dans le fichier
        File.WriteAllText("reservation.json", json);
        Console.WriteLine("Fichier reservation.json généré avec succès !");
    }


}