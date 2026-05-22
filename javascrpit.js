

// L'adresse du serveur (API)
const API_BASE_URL = "https://can.iutrs.unistra.fr/api";



// Fonction pour aller chercher les données sur le serveur
const fetchAPI = async (endpoint) => {
    try {
        const url = API_BASE_URL + endpoint;
        console.log("Chargement de : " + url); // Pour vérifier si ça marche

        const response = await fetch(url);

        // Si le serveur a un problème
        if (!response.ok) {
            throw new Error('Erreur de connexion');
        }

        const data = await response.json();
        return data;

    } catch (error) {
        console.error('Erreur technique :', error);
        alert("Impossible de récupérer les données.");
        return null;
    }
};


// 1. PAGE EMBARQUEMENT

const initEmbarquement = async () => {
   console.log("Mode test : lecture de adams.json");
    const response = await fetch("reservation.json"); // On lit le fichier local
    const data = await response.json();
    if (!data) return;

    // Création de la liste des tickets
    let tickets = []; 

    // Ajout des passagers
    if (data.passagers) {
        for (let i = 0; i < data.passagers.length; i++) {
            let p = data.passagers[i];
            tickets.push({
                type: 'Passager',
                nom: p.nom,
                categorie: p.categorie || "Standard", 
                prix: p.prix
            }); 
        }
    }

    // Ajout des véhicules
    if (data.vehicules) {
        for (let i = 0; i < data.vehicules.length; i++) {
            let v = data.vehicules[i];
            tickets.push({
                type: 'Véhicule',
                nom: v.categorie, // Le nom est la catégorie pour un véhicule
                categorie: "Véhicule",
                prix: v.prix
            });
        }
    }

    // Gestion de l'affichage
    let indexActuel = 0;
    const container = document.getElementById('ticket-container');
    const btnPrev = document.getElementById('btn-prev');
    const btnNext = document.getElementById('btn-next');

    // Fonction pour afficher un ticket précis
    function afficherTicket(index) {
        if (index < 0 || index >= tickets.length) return;

        let item = tickets[index];
        
        let html = `
            <div class="ticket-box">
                <h3>${item.type} - Billet ${index + 1}/${tickets.length}</h3>
                <p><strong>Nom :</strong> ${item.nom}</p>
                <p><strong>Catégorie :</strong> ${item.categorie}</p>
                <p><strong>Prix :</strong> ${item.prix} €</p>
                <p><strong>Trajet :</strong> ${data.liaison}</p>
                <p><strong>Date :</strong> ${data.date} à ${data.heure}</p>
            </div>
        `;
        container.innerHTML = html;
    }

    // Clic bouton Précédent
    if (btnPrev) {
        btnPrev.addEventListener('click', function() {
            if (indexActuel > 0) {
                indexActuel--; 
                afficherTicket(indexActuel);
            }
        });
    }

    // Clic bouton Suivant
    if (btnNext) {
        btnNext.addEventListener('click', function() {
            if (indexActuel < tickets.length - 1) {
                indexActuel++;
                afficherTicket(indexActuel);
            }
        });
    }

    // Lancement affichage
    if (tickets.length > 0) afficherTicket(0);
    else container.innerHTML = "<p>Aucun billet.</p>";
};

// 2. PAGE FACTURE


const initFacture = async () => {
    console.log("Mode test : lecture de reservation.json");
    const response = await fetch("reservation.json");
    const data = await response.json();
    if (!data) return;

    // Remplissage des infos client
    // On utilise getElementById pour trouver les zones de texte
    if(document.getElementById('facture-ref')) 
        document.getElementById('facture-ref').innerText = data.numeroReservation;
    
    if(document.getElementById('facture-client'))
        document.getElementById('facture-client').innerText = data.nomClient || "Client";
    
    if(document.getElementById('facture-date'))
        document.getElementById('facture-date').innerText = data.date;

    // Fonction pour remplir un tableau HTML (Passagers ou Véhicules)
    function remplirTableau(liste, idTableau) {
        const tbody = document.getElementById(idTableau);
        // Si l'ID n'existe pas dans le HTML, on arrête pour éviter le bug
        if (!tbody) {
            console.error("Attention : L'ID " + idTableau + " n'existe pas dans facture.html !");
            return 0;
        }

        tbody.innerHTML = ""; // On vide le tableau
        let total = 0;

        // On compte les quantités par catégorie (ex: 2 adultes)
        let compteurs = {}; 

        for (let i = 0; i < liste.length; i++) {
            let item = liste[i];
            let cat = item.categorie || "Autre";
            let prix = item.prix || 0;

            if (!compteurs[cat]) {
                compteurs[cat] = { qte: 0, prix: prix };
            }
            compteurs[cat].qte++;
        }

        // On génère les lignes HTML
        for (let cat in compteurs) {
            let infos = compteurs[cat];
            let sousTotal = infos.qte * infos.prix;
            total += sousTotal;

            let ligne = `
                <tr>
                    <td>${cat}</td>
                    <td>${infos.qte}</td>
                    <td>${infos.prix} €</td>
                    <td>${sousTotal.toFixed(2)} €</td>
                </tr>
            `;
            tbody.innerHTML += ligne;
        }
        return total;
    }

    // Calculs et affichage
    let totalPax = remplirTableau(data.passagers || [], 'tbody-passagers');
    let totalVeh = remplirTableau(data.vehicules || [], 'tbody-vehicules');

    // Mise à jour des totaux en bas de page
    if(document.getElementById('total-pax'))
        document.getElementById('total-pax').innerText = totalPax.toFixed(2) + ' €';
    
    if(document.getElementById('total-veh'))
        document.getElementById('total-veh').innerText = totalVeh.toFixed(2) + ' €';
    
    if(document.getElementById('grand-total'))
        document.getElementById('grand-total').innerText = (totalPax + totalVeh).toFixed(2) + ' €';
};


// 3. PAGE TABLEAU DE BORD 


const initDashboard = async () => {
    const data = await fetchAPI("/traversees");
    if (!data) return;

    const container = document.getElementById('dashboard-container');
    container.innerHTML = ''; 

    for (let i = 0; i < data.length; i++) {
        let t = data[i];

        // Calcul des pourcentages
        let capPax = t.capacitePassagers || 1; 
        let capVeh = t.capaciteVehicules || 1;
        
        let pctPax = Math.round((t.nbPassagers / capPax) * 100);
        let pctVeh = Math.round((t.nbVehicules / capVeh) * 100);

        // Choix de la couleur selon le sujet
        function getCouleur(p) {
            if (p > 100) return 'jaune-fluo';
            if (p >= 75) return 'rouge';
            if (p >= 50) return 'orange';
            return 'vert';
        


const initStats = async () => {
    const data = await fetchAPI("/stats");
    if (!data) return;

    let ca = data.totalCA || data.chiffreAffairesTotal || 0;
    let pax = data.totalPax || data.nombrePassagersTotal || 0;

    // Affichage avec formatage (ex: 1 200 €)
    if(document.getElementById('stat-ca'))
        document.getElementById('stat-ca').innerText = ca.toLocaleString() + ' €';
    
    if(document.getElementById('stat-pax'))
        document.getElementById('stat-pax').innerText = pax.toLocaleString();
};


// LANCEMENT AUTOMATIQUE


document.addEventListener('DOMContentLoaded', function() {
    console.log("Démarrage du script...");

    // On regarde quel élément existe sur la page pour savoir quoi lancer
    
    if (document.getElementById('ticket-container')) {
        initEmbarquement();
    }
    
    if (document.getElementById('facture-ref') || document.getElementById('tbody-passagers')) {
        initFacture();
    }
    
    if (document.getElementById('dashboard-container')) {
        initDashboard();
    }
    
    if (document.getElementById('stat-ca')) {
        initStats();
    }
});
